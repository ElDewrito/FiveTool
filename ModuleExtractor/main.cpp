// This is a .module extractor that was hacked together in about a day.
// It seems to work with all of the .module files shipped with H5F.
// It will be replaced with a cleaner C# solution eventually.

#include "blam_module.hpp"
#include "io.hpp"

#include <iostream>
#include <fstream>
#include <memory>
#include <filesystem>
#include "zlib/zlib.h"

using namespace std::experimental;

bool checkZlibResult(int result);
void displayError(const std::string& error);

int main(int argc, char *argv[])
{
	if (argc != 2 && argc != 3)
	{
		std::cout << "Usage: " << argv[0] << " <filename> [output dir]";
		return 1;
	}

	filesystem::path modulePath = argv[1];
	filesystem::path outputDir = (argc >= 3) ? argv[2] : modulePath.parent_path() / modulePath.stem();
	outputDir = filesystem::absolute(outputDir); // hack: outputDir needs to be absolute or else the path functions get screwed up if there's a colon in a filename
	std::ifstream moduleFile(modulePath.native(), std::ios::binary);
	if (!moduleFile)
	{
		displayError("Failed to open " + modulePath.string() + " for reading");
		return 1;
	}
	filesystem::create_directory(outputDir);

	std::cout << "Reading header..." << std::endl;

	BlamModuleHeader header;
	ReadStruct(moduleFile, header);
	if (header.magic != BlamModuleHeaderMagic)
	{
		displayError("Invalid module header");
		return 1;
	}
	if (header.version != LatestBlamModuleVersion)
	{
		displayError("Invalid module version");
		return 1;
	}
	WriteStruct(header, outputDir / "header");

	std::cout << "Reading file list..." << std::endl;

	auto files = std::make_unique<BlamModuleFile[]>(header.numFiles);
	ReadArray(moduleFile, header.numFiles, &files[0]);
	WriteArray(&files[0], header.numFiles, outputDir / "files");

	std::cout << "Reading file names..." << std::endl;

	auto fileNames = std::make_unique<char[]>(header.fileNamesSize);
	ReadArray(moduleFile, header.fileNamesSize, &fileNames[0]);
	WriteArray(&fileNames[0], header.fileNamesSize, outputDir / "names");

	std::cout << "Reading resource list..." << std::endl;

	auto resources = std::make_unique<uint32_t[]>(header.numResources);
	ReadArray(moduleFile, header.numResources, &resources[0]);
	WriteArray(&resources[0], header.numResources, outputDir / "resources");

	std::cout << "Reading compressed block index..." << std::endl;

	auto compressedBlocks = std::make_unique<BlamModuleCompressedBlock[]>(header.numCompressedBlocks);
	ReadArray(moduleFile, header.numCompressedBlocks, &compressedBlocks[0]);
	WriteArray(&compressedBlocks[0], header.numCompressedBlocks, outputDir / "block_index");

	auto compressedDataStart = static_cast<int64_t>(moduleFile.tellg());

	for (auto i = 0; i < header.numFiles; i++)
	{
		auto& file = files[i];
		auto name = &fileNames[file.nameOffset];

		auto progress = i * 100 / header.numFiles;
		std::cout << "[" << progress << "%] Extracting " << name << "..." << std::endl;

		std::unique_ptr<uint8_t[]> uncompressedData;
		uLongf uncompressedSize = file.totalUncompressedSize;
		if (uncompressedSize == 0)
		{
			// empty file
			uncompressedData = std::make_unique<uint8_t[]>(0);
			uncompressedSize = 0;
		}
		else if (file.compressedBlockCount > 0)
		{
			// decompress the file in blocks
			uncompressedData = std::make_unique<uint8_t[]>(file.totalUncompressedSize);
			for (auto j = 0; j < file.compressedBlockCount; j++)
			{
				auto& block = compressedBlocks[file.firstCompressedBlockIndex + j];
				auto uncompressedOffset = block.uncompressedOffset;
				auto fileOffset = compressedDataStart + file.compressedOffset + block.compressedOffset;
				moduleFile.seekg(fileOffset, std::ios::beg);
				if (block.compressed)
				{
					auto compressedSize = block.compressedSize;
					auto compressedData = std::make_unique<uint8_t[]>(compressedSize);
					ReadData(moduleFile, compressedSize, &compressedData[0]);
					uLongf actualUncompressedSize = block.uncompressedSize;
					auto result = uncompress(&uncompressedData[uncompressedOffset], &actualUncompressedSize, &compressedData[0], compressedSize);
					if (!checkZlibResult(result))
						return 1;
				}
				else
				{
					ReadData(moduleFile, block.uncompressedSize, &uncompressedData[uncompressedOffset]);
				}
			}
		}
		else
		{
			// decompress using totalUncompressedSize and totalCompressedSize
			auto compressedSize = file.totalCompressedSize;
			uncompressedData = std::make_unique<uint8_t[]>(uncompressedSize);
			moduleFile.seekg(compressedDataStart + file.compressedOffset, std::ios::beg);
			if (compressedSize != uncompressedSize)
			{
				auto compressedData = std::make_unique<uint8_t[]>(compressedSize);
				ReadData(moduleFile, compressedSize, &compressedData[0]);
				auto result = uncompress(&uncompressedData[0], &uncompressedSize, &compressedData[0], compressedSize);
				if (!checkZlibResult(result))
					return 1;
			}
			else
			{
				ReadData(moduleFile, uncompressedSize, &uncompressedData[0]);
			}
		}

		try
		{
			auto tagPath = SanitizeFileName(outputDir / name);
			filesystem::create_directories(tagPath.parent_path());
			WriteData(&uncompressedData[0], uncompressedSize, tagPath);
		}
		catch (const std::exception& e)
		{
			displayError(e.what());
			return 1;
		}
	}
	return 0;
}

bool checkZlibResult(int result)
{
	if (result != Z_OK)
	{
		displayError("ZLib returned " + std::to_string(result) + " (" + zError(result) + ")");
		return false;
	}
	return true;
}

void displayError(const std::string& error)
{
	std::cerr << "Extraction failed: " << error << std::endl;
	std::cout << "Press Enter to quit..." << std::endl;
	std::cin.ignore();
}