DefineHelp ("File", "ChooseGameFolder", {
	shortDescription = "Choose a different game folder to use",
	longDescription = "Asks the user to choose where their game files are installed.\nAll functions which accept file paths will use this folder as a base.",
	returns = "true if the user selected a folder",
	examples = {
		"ChooseGameFolder()",
	},
});