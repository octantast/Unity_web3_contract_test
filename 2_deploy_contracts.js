var SimpleVoting = artifacts.require("./SimpleVoting.sol"); // Replace with the name of your contract

module.exports = function( deployer ) {

	// Replace with your preferred gas values
	const gasLimit = 5000000;
	const gasPrice = 1000000000;

	deployer.deploy( SimpleVoting );
}
