local helpByFunction = {}
local helpByGroup = {}

local GetKeysSorted
local GetArgsSignature
local PrintFunctionHelp
local PrintGroupHelp
local PrintAllHelp

-- help table keys:
--   shortDescription - Short function description
--   longDescription - (Optional) Long function description
--   args - (Optional) Array of { argument, description } pairs
--   returns - (Optional) Return value description
--   examples - (Optional) Array of examples on how to use the function
function DefineHelp (groupName, fnName, fn, help)
	help.name = fnName
	helpByFunction[fn] = help
	if helpByGroup[groupName] == nil then
		helpByGroup[groupName] = {}
	end
	local groupHelp = helpByGroup[groupName]
	groupHelp[fnName] = help
end

function Help (search)
	if type(search) == "function" then
		help = helpByFunction[search]
		if help ~= nil then
			PrintFunctionHelp(help)
		else
			Color("Yellow")
			io.write("Unable to find help")
		end
	elseif type(search) == "string" then
		group = helpByGroup[search]
		if group ~= nil then
			PrintGroupHelp(search, group)
		else
			Color("Yellow")
			io.write("Unable to find help for group \"" .. search .. "\"\n")
		end
	else
		PrintAllHelp()
	end
end

DefineHelp ("Core", "Help", Help, {
	shortDescription = "Get help about a function or group of functions",
	args = {
		{ "search", "(Optional) The function or group name" },
	},
	examples = {
		"Help()",
		"Help(LoadModule)",
		"Help(\"Modules\")",
	},
});

GetKeysSorted = function (t)
	local keys = {}
	for key, _ in pairs(t) do
		table.insert(keys, key)
	end
	table.sort(keys)
	return keys
end

GetArgsSignature = function (help)
	local signature = ""
	local optionalCount = 0
	if help.args ~= nil then
		for argIndex, arg in ipairs(help.args) do
			local name, description = unpack(arg)
			if string.find(description, "^%(Optional%)") ~= nil then
				-- If an argument description begins with "(Optional)", then enclose it in []
				signature = signature .. "["
				optionalCount = optionalCount + 1
			end
			if argIndex > 1 then
				signature = signature .. ", "
			end
			signature = signature .. name
		end
	end
	for i = 1, optionalCount do
		signature = signature .. "]"
	end
	return signature
end

PrintFunctionHelp = function (help)
	Color("White")
	io.write("\n", help.name, "(", GetArgsSignature(help), ")\n\n")
	Color("Gray")
	if help.longDescription ~= nil then
		io.write(help.longDescription, "\n")
	else
		io.write(help.shortDescription, "\n")
	end
	if help.args ~= nil and #help.args > 0 then
		Color("Cyan")
		io.write("\nArguments:\n\n")
		for i, arg in ipairs(help.args) do
			local name, description = unpack(arg)
			Color("White")
			io.write("  ", name)
			Color("Gray")
			io.write(" - ", description, "\n")
		end
	end
	if help.returns ~= nil then
		Color("Cyan")
		io.write("\nReturns:\n\n")
		Color("Gray")
		io.write("  ", help.returns, "\n")
	end
	if help.examples ~= nil and #help.examples > 0 then
		Color("Cyan")
		io.write("\nExamples:\n")
		Color("Gray")
		for i, example in ipairs(help.examples) do
			io.write("\n  ", example, "\n")
		end
	end
end

PrintGroupHelp = function (groupName, group)
	Color("Cyan")
	io.write("\n", groupName, " Functions:\n\n")
	local functionNames = GetKeysSorted(group)
	for _, name in ipairs(functionNames) do
		local help = group[name]
		Color("White")
		io.write("  ", name, "(")
		Color("DarkGray")
		io.write(GetArgsSignature(help))
		Color("White")
		io.write(")")
		Color("Gray")
		io.write(" - ", help.shortDescription, "\n")
	end
end

PrintAllHelp = function ()
	io.write("\nUse Help(FunctionName) or Help(\"Group Name\") to get more specific help.\n")
	groups = GetKeysSorted(helpByGroup)
	for i, group in ipairs(groups) do
		PrintGroupHelp(group, helpByGroup[group])
	end
end