#include "io.hpp"

#include <algorithm>

using namespace std::experimental;

void ReadData(std::ifstream& stream, int64_t size, uint8_t* out)
{
	stream.read(reinterpret_cast<char*>(out), size);
}

void WriteData(const uint8_t* data, int64_t size, const filesystem::path& path)
{
	std::ofstream file(path.native(), std::ios::binary | std::ios::trunc);
	file.write(reinterpret_cast<const char*>(data), size);
}

namespace
{
	const std::string InvalidFileNameChars = ":*?\"<>|";
}

filesystem::path SanitizeFileName(const filesystem::path& path)
{
	auto filename = path.filename().native();
	std::transform(filename.begin(), filename.end(), filename.begin(), [](char ch)
	{
		return (InvalidFileNameChars.find(ch) == std::string::npos) ? ch : '_';
	});
	return path.parent_path() / filename;
}