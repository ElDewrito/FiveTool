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
		std::cerr << "Failed to open " << modulePath << " for reading." << std::endl;
		return 1;
	}
	filesystem::create_directory(outputDir);

	std::cout << "Reading header..." << std::endl;

	BlamModuleHeader header;
	ReadStruct(moduleFile, header);
	if (header.magic != BlamModuleHeaderMagic)
	{
		std::cerr << "Invalid module header" << std::endl;
		return 1;
	}
	if (header.version != LatestBlamModuleVersion)
	{
		std::cerr << "Invalid module version" << std::endl;
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

	std::cout << "Reading root tag list..." << std::endl;

	auto rootTags = std::make_unique<uint32_t[]>(header.numRootTags);
	ReadArray(moduleFile, header.numRootTags, &rootTags[0]);
	WriteArray(&rootTags[0], header.numRootTags, outputDir / "root_tags");

	std::cout << "Reading compressed block index..." << std::endl;

	auto compressedBlocks = std::make_unique<BlamModuleCompressedBlock[]>(header.numCompressedBlocks);
	ReadArray(moduleFile, header.numCompressedBlocks, &compressedBlocks[0]);
	WriteArray(&compressedBlocks[0], header.numCompressedBlocks, outputDir / "block_index");

	auto compressedDataStart = static_cast<size_t>(moduleFile.tellg());

	for (auto i = 0; i < header.numFiles; i++)
	{
		auto& file = files[i];
		auto name = &fileNames[file.nameOffset];

		std::cout << "Extracting " << name << "..." << std::endl;

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
				auto compressedSize = block.compressedSize;
				auto compressedData = std::make_unique<uint8_t[]>(compressedSize);
				auto fileOffset = compressedDataStart + file.compressedOffset + block.compressedOffset;
				moduleFile.seekg(fileOffset, std::ios::beg);
				ReadData(moduleFile, compressedSize, &compressedData[0]);

				auto uncompressedOffset = block.uncompressedOffset;
				uLongf actualUncompressedSize = block.uncompressedSize;
				uncompress(&uncompressedData[uncompressedOffset], &actualUncompressedSize, &compressedData[0], compressedSize);
			}
		}
		else
		{
			// decompress using totalUncompressedSize and totalCompressedSize
			auto compressedSize = file.totalCompressedSize;
			auto compressedData = std::make_unique<uint8_t[]>(compressedSize);
			moduleFile.seekg(compressedDataStart + file.compressedOffset, std::ios::beg);
			ReadData(moduleFile, compressedSize, &compressedData[0]);

			uncompressedData = std::make_unique<uint8_t[]>(uncompressedSize);
			uncompress(&uncompressedData[0], &uncompressedSize, &compressedData[0], compressedSize);
		}

		auto tagPath = SanitizeFileName(outputDir / name);
		filesystem::create_directories(tagPath.parent_path());
		WriteData(&uncompressedData[0], uncompressedSize, tagPath);
	}
	return 0;
}
