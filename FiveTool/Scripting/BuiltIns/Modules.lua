local loadedModules = {} -- Modules keyed by ID string

local GetModuleFromEntry

function LoadModule(path)
	if path == nil then
		path = ChooseFile("module", "Open Module File")
		if path == nil then
			return nil
		end
	end
	local id = Hex(AusarModule.ReadId(path))
	if loadedModules[id] == nil then
		loadedModules[id] = AusarModule.LoadFromFile(path)
	end
	return loadedModules[id]
end

DefineHelp("Module", "LoadModule", {
	shortDescription = "Load a module file",
	longDescription = "Loads a module file.\nIts contents will be made accessible to other functions.\nIf a path is not provided, the user will be asked to browse for one.",
	args = {
		{ "path", "(Optional) The path to the module file to load" },
	},
	returns = "The loaded module object, or nil if no path was given and the user cancelled.",
	examples = {
		"LoadModule([[deploy\\any\\levels\\globals-rtx-1.module]])",
	},
});

function GetModuleEntry(nameOrId)
	for id, module in pairs(loadedModules) do
		local entry
		if type(nameOrId) == "number" then
			entry = module.GetEntryByGlobalTagId(nameOrId)
		else
			entry = module.GetEntryByName(nameOrId)
		end
		if entry ~= nil then
			return entry
		end
	end
	return nil
end

DefineHelp("Module", "GetModuleEntry", {
	shortDescription = "Look up a module entry by name or global ID",
	longDescription = "Looks up a module entry by name or global ID.\nIf a name is supplied, it must match exactly.",
	args = {
		{ "nameOrId", "The full name or global ID of the entry to get" },
	},
	returns = "The module entry object if it is loaded, or nil otherwise.",
	examples = {
		"ar = GetModuleEntry([[objects\\weapons\\rifle\\storm_assault_rifle\\storm_assault_rifle.weapon]])",
		"ar = GetModuleEntry(0x8595)",
	},
});

function FindModuleEntry(name)
	for id, module in pairs(loadedModules) do
		for i, entry in ipairs(module.Entries) do
			if string.find(entry.Name, name, 1, true) ~= nil then
				return entry
			end
		end
	end
	return nil
end

DefineHelp("Module", "FindModuleEntry", {
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

function FindModuleEntries(name)
	local entries = {}
	for id, module in pairs(loadedModules) do
		for i, entry in ipairs(module.Entries) do
			if string.find(entry.Name, name, 1, true) ~= nil then
				table.insert(entries, entry)
			end
		end
	end
	return entries
end

DefineHelp("Module", "FindModuleEntries", {
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

function ExtractModuleEntry(entry, path, section)
	if type(entry) ~= "userdata" then
		entry = GetModuleEntry(entry)
		if entry == nil then
			return false
		end
	end
	module = GetModuleFromEntry(entry)
	if module == nil then
		return false
	end
	if path == nil then
		path = ChooseNewFile()
		if path == nil then
			return false
		end
	end
	if section == nil then
		section = "all"
	end
	module.ExtractEntry(entry, path, section)
	return true
end

DefineHelp("Module", "ExtractModuleEntry", {
	shortDescription = "Extract an entry from a module",
	longDescription = "Extracts all or part of an entry from a module.\nThe module must have been loaded with LoadModule().",
	returns = "false if the entry was not found or a path was not selected, true otherwise",
	args = {
		{ "entry", "The name, global ID, or entry object of the entry to extract" },
		{ "path", "(Optional) The relative path or path token to extract the entry to" },
		{ "section", "(Optional) The section to extract (header|tag|resource|all)" },
	},
	examples = {
		"ExtractModuleEntry(0x8595)",
		"ExtractModuleEntry(0x8595, \"assault_rifle.weapon\")",
		"ExtractModuleEntry(0x8595, \"assault_rifle.header.bin\", \"header\")",
	},
})

GetModuleFromEntry = function (entry)
	-- does this need to be faster?
	for id, module in pairs(loadedModules) do
		if module.ContainsEntry(entry) then
			return module
		end
	end
	return nil
end