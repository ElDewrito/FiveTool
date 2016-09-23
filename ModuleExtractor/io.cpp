#include "io.hpp"

#include <algorithm>

using namespace std::experimental;

namespace
{
	const std::string InvalidFileNameChars = ":*?\"<>|";
	const std::wstring LongPathPrefix = L"\\\\?\\";
}

void ReadData(std::ifstream& stream, int64_t size, uint8_t* out)
{
	stream.read(reinterpret_cast<char*>(out), size);
}

bool WriteData(const uint8_t* data, int64_t size, const filesystem::path& path)
{
	std::ofstream file(MakeLongPath(path).native(), std::ios::binary | std::ios::trunc);
	if (!file)
		return false;
	file.write(reinterpret_cast<const char*>(data), size);
	return true;
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

filesystem::path MakeLongPath(const filesystem::path& path)
{
	filesystem::path result = LongPathPrefix;
	result += path;
	return result;
}

void CreateDirectories(const filesystem::path& path)
{
	// filesystem::create_directories doesn't have long path support...
	if (filesystem::exists(MakeLongPath(path)))
		return;
	CreateDirectories(path.parent_path());
	filesystem::create_directory(MakeLongPath(path));
}