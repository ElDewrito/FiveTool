local loadedModules = {} -- Modules keyed by path

function LoadModule (path)
	if loadedModules[path] == nil then
		loadedModules[path] = AusarModule.LoadFromFile(path)
	end
	return loadedModules[path]
end

DefineHelp ("Module", "LoadModule", {
	shortDescription = "Load a module file",
	longDescription = "Loads a module file.\nIts contents will be made accessible to other functions.",
	args = {
		{ "path", "The path to the module file to load" },
	},
	returns = "The loaded module object.",
	examples = {
		"LoadModule([[deploy\\any\\levels\\globals-rtx-1.module]])",
	},
});

function GetModuleEntry (name)
	for path, module in pairs(loadedModules) do
		local entry = module.GetEntryByName(name)
		if entry ~= nil then
			return entry
		end
	end
	return nil
end

DefineHelp ("Module", "GetModuleEntry", {
	shortDescription = "Look up a module entry by name",
	longDescription = "Looks up a module entry by name.\nThe name must match exactly.",
	args = {
		{ "name", "The full name of the entry to get" },
	},
	returns = "The module entry object if it exists, or nil otherwise.",
	examples = {
		"ar = GetModuleEntry([[objects\\weapons\\rifle\\storm_assault_rifle\\storm_assault_rifle.weapon]])",
	},
});

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

DefineHelp ("Module", "FindModuleEntry", {
	shortDescription = "Find the first module entry matching a string",
	longDescription = "Finds the first module entry matching a string.\nThe string can be anywhere in an entry's name.",
	returns = "The module entry object if found, or nil otherwise.",
	args = {
		{ "name", "The string to search for" },
	},
	examples = {
		"ar = FindModuleEntry(\"storm_assault_rifle.weapon\")",
	},
});

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

DefineHelp ("Module", "FindModuleEntries", {
	shortDescription = "Find all module entries matching a string",
	longDescription = "Finds all module entries matching a string.\nThe string can be anywhere in an entry's name.",
	returns = "An array of all found entries.",
	args = {
		{ "name", "The string to search for" },
	},
	examples = {
		"bipeds = FindModuleEntries(\".biped\")",
	},
});