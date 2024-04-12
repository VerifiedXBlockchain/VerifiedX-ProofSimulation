async void Main()
{
	ConcurrentDictionary<string, Account> Validators = new ConcurrentDictionary<string, Account>();
	ConcurrentDictionary<string, int> ValidatorWins = new ConcurrentDictionary<string, int>();
	
	//Creates 1000 random RBX Addresses
	for(var i = 0; i < 1000; i++)
	{
		var account = AccountData.CreateNewAccount(true);
		Validators.TryAdd(account.Address, account);
	}
	
	//Runs simulations for each block with all 1k address and stores winners
	for(var i = 0; i < 10000000; i++)
	{
		var proofs = new Dictionary<string, uint>();
		foreach(var validator in Validators)
		{
			var address = validator.Key;
			var pubKey = validator.Value.PublicKey;
			var height = i;
			
			var result = await ProofUtility.CreateProof(address, pubKey, height);
			proofs.Add(address, result.Item1);
		}
		
		var winner = proofs.OrderBy(x => x.Value).FirstOrDefault();
		
		var locate = ValidatorWins.TryGetValue(winner.Key, out int value);
		
		if(locate)
		{
			ValidatorWins[winner.Key] = value + 1;
		}
		else
		{
			ValidatorWins.TryAdd(winner.Key, 1);
		}
	}
	
	string filePath = @"D:\ValidatorWins.csv"; //update to your file path
	
	WriteDictionaryToCSV(ValidatorWins, filePath);
	ValidatorWins.Dump();
}

// You can define other methods, fields, classes and namespaces here
static void WriteDictionaryToCSV(ConcurrentDictionary<string, int> dictionary, string filePath)
{
	// Open or create the CSV file for writing
	using (StreamWriter writer = new StreamWriter(filePath))
	{
		// Write the header row
		writer.WriteLine("Validator, Wins");

		// Write each key-value pair as a row in the CSV file
		foreach (var kvp in dictionary)
		{
			// Escape special characters if needed
			string validator = kvp.Key.Replace(",", "");
			int wins = kvp.Value;

			// Write the row to the CSV file
			writer.WriteLine($"{validator}, {wins}");
		}
	}

	Console.WriteLine($"CSV file has been written to: {filePath}");
}