local loadedModules = {} -- Modules keyed by path

function LoadModule (path)
	if loadedModules[path] == nil then
		loadedModules[path] = Module.LoadFromFile(path)
	end
	return loadedModules[path]
end

function GetModuleEntry (name)
	for path, module in pairs(loadedModules) do
		local entry = module.GetEntryByName(name)
		if entry ~= nil then
			return entry
		end
	end
	return nil
end

function FindModuleEntry (name)
	for path, module in pairs(loadedModules) do
		for i, entry in ipairs(module.Entries) do
			if string.find(entry.Name, name, 1, true) ~= nil then
				return entry
			end
		end
	end
	return nil
end

function FindModuleEntries (name)
	local entries = {}
	for path, module in pairs(loadedModules) do
		for i, entry in ipairs(module.Entries) do
			if string.find(entry.Name, name, 1, true) ~= nil then
				table.insert(entries, entry)
			end
		end
	end
	return entries
end