using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;

public class SmartContractReader : MonoBehaviour
{
    private string contractAddress; // Address of the deployed contract
	private string contractABI; // Contract ABI as a string
	private string rpcUrl; // URL of local RPC node

	public string senderAddress; // Account for sending the transaction
	public int health; // Input value
	public int healthAverage; // Average value for reading

	private Web3 web3; // Instance of the Nethereum Web3 class
	private Contract contract; // Instance of the Nethereum Contract class

	private void Start()
	{
		// Enter your values
		contractAddress = "0xCb4C96b60eb065f7E9b31C2b2b93413b03CF49f5";
        contractABI = @"[
    {
        ""stateMutability"": ""nonpayable"",
        ""type"": ""fallback""
    },
    {
        ""inputs"": [
            {
                ""internalType"": ""address"",
                ""name"": """",
                ""type"": ""address""
            }
        ],
        ""name"": ""hasVoted"",
        ""outputs"": [
            {
                ""internalType"": ""bool"",
                ""name"": """",
                ""type"": ""bool""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""proposedHealth"",
        ""outputs"": [
            {
                ""internalType"": ""uint256"",
                ""name"": """",
                ""type"": ""uint256""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""readHealth"",
        ""outputs"": [
            {
                ""internalType"": ""uint256"",
                ""name"": """",
                ""type"": ""uint256""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""sum"",
        ""outputs"": [
            {
                ""internalType"": ""uint256"",
                ""name"": """",
                ""type"": ""uint256""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [],
        ""name"": ""totalVotes"",
        ""outputs"": [
            {
                ""internalType"": ""uint256"",
                ""name"": """",
                ""type"": ""uint256""
            }
        ],
        ""stateMutability"": ""view"",
        ""type"": ""function""
    },
    {
        ""inputs"": [
            {
                ""internalType"": ""uint256"",
                ""name"": ""health"",
                ""type"": ""uint256""
            }
        ],
        ""name"": ""voteForHealth"",
        ""outputs"": [],
        ""stateMutability"": ""nonpayable"",
        ""type"": ""function""
    },
    {
        ""stateMutability"": ""payable"",
        ""type"": ""receive""
    }
]";
        rpcUrl = "http://localhost:7545";

		web3 = new Web3(rpcUrl);
        contract = web3.Eth.GetContract(contractABI, contractAddress);

		// Pre-read from contract
		StartCoroutine(PreReadFromContract());
    }

	// Method for voting for health, OnClick() event calls it
	public async void VoteForHealth()
    {
		// Getting function from the contract
		var voteForHealthFunction = contract.GetFunction("voteForHealth");

        try
		{
			// Creating a transaction to call the voting function
			var transactionInput = voteForHealthFunction.CreateTransactionInput(senderAddress, health);

			// Setting the gas limit, gas price and transaction value
			transactionInput.Gas = new HexBigInteger(200000); // Example of a gas limit
			transactionInput.GasPrice = new HexBigInteger(20000000000); // Example of gas price in Wei
			transactionInput.Value = new HexBigInteger(0); // Value in Wei (zero value for functions without ether transfer)

			// Sending the transaction
			var transactionHash = await web3.Eth.Transactions.SendTransaction.SendRequestAsync(transactionInput);
            Debug.Log($"Transaction Hash: {transactionHash}");

			// Reading data from the contract after voting
			await ReadFromContract();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }

	// Asynchronous method for reading data from the contract
	private async Task ReadFromContract()
	{
		// Getting function from the contract
		var proposedHealthFunction = contract.GetFunction("readHealth");

        try
        {
			// Getting value from the function
			var result = await proposedHealthFunction.CallAsync<BigInteger>();
            Debug.Log($"Proposed Health: {result}");

			// Updating the healthAverage variable with the received value
			healthAverage = (int)result;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }

	// Coroutine for pre-reading from the contract
	private IEnumerator PreReadFromContract()
    {
        yield return ReadFromContract();
    }
}
