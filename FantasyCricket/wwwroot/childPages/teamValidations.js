/************************************************************
 * @function       validator - Validates the team structure
 * @param          {Object} dataObj - Team to be validated
 * @return         {Object} validationResult
 *                 If Not Success, It returns the error object with exact error message
 ************************************************************/
let validator = function(dataObj) {
    let self = this;
    self.data = dataObj;
    self.runValidations = function() {
        let response = { isError : false };
        // Classiication per role
        let roles = {
            canBat: { count: 0, minAllowed: 6, error: "You need to have atleast 6 Batsmen" },
            canBowl: { count: 0, minAllowed: 5, error: "You need to have atleast 5 Bowlers" },
            canKeep: { count: 0, minAllowed: 1, error: "You need to have atleast 1 Wicket Keeper" },
            canPlay: {count: self.data.length, minAllowed: 11, maxAllowed: 11, error: "You need to have 11 players" }
        };
        // Check for Max players in a team
        let playersInATeam = {};
        let maxPlayerFromATeamAllowed = { count: 6, error: "We can not have more than 6 players from single team" };
        // Check for budget
        let maxAllowedBudget = { count: 1000, error: "We can not have budget more than 1000" };
        let totalBudget = 0;      
        // Loop through the players list within the team
        // And determine the roles each player play
        self.data.map(function(playerData) {
            switch(playerData.Role) {
                case "BAT": roles.canBat.count++; break;
                case "BOWL": roles.canBowl.count++; break;
                case "ALL": roles.canBowl.count++; roles.canBat.count++; break;
                case "WK": roles.canKeep.count++; roles.canBat.count++; break;
                default: break;
            }
            playersInATeam[playerData.TeamName] = playersInATeam[playerData.TeamName] || 0;
            playersInATeam[playerData.TeamName]++;
            totalBudget += playerData.Cost;
        });
        
        response.budgetLeft = maxAllowedBudget.count - totalBudget;
        // Check if we have minimum players for a role
        for(let key in roles) {
            if(roles[key].count < roles[key].minAllowed) pushError(roles[key], response);
            if(roles[key].maxAllowed && (roles[key].maxAllowed < roles[key].count)) pushError(roles[key], response);
        }           
        // Check if budget has exceeded
        (maxAllowedBudget.count < totalBudget) && pushError(maxAllowedBudget, response);
        // Check if maximum allowed players in a team exceeds limit
        for(let tKey in playersInATeam) {
            (playersInATeam[tKey] > maxPlayerFromATeamAllowed.count) && pushError(maxPlayerFromATeamAllowed, response);
        }
        return response;
    };
    
    function pushError(obj, response) {
        response.isError = true;
        response.error = obj.error;
    }
};