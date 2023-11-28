// SimpleVoting.sol
pragma solidity ^0.8.0;

contract SimpleVoting {
    uint256 public totalVotes; // Total number of votes
    uint256 public sum; // Sum of all votes
    uint256 public proposedHealth; // Calculated average vote
    mapping(address => bool) public hasVoted; // A list that tracks whether a specific address has voted

    // Function that can accept ether (payments)
    receive() external payable {}

    // A function that can be called if the contract receives ether without calling the function
    fallback() external {}

    // Function for voting for health
    function voteForHealth(uint256 health) external {
        // Lower and upper limits of the input value
        require(health > 0);

        // Check that the address has not yet voted
        require(!hasVoted[msg.sender]);

        // Increase the total number of votes, mark the address as voted and increase the total votes
        totalVotes++;
        hasVoted[msg.sender] = true;
        sum +=health;        

        // Calculate the average health value
        if(totalVotes != 0)
        {
        proposedHealth = sum / totalVotes;
        }
    }

    // Function to read the current average health value without changing the contract state
    function readHealth() external view returns (uint256) {
        return proposedHealth;
    }
}
