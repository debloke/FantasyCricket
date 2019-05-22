"use strict";
var ALL_PLAYERS = [];
/****************************************************************************
* @function       Page Load                                                 *
* @brief          Triggeres when page is loaded                             *
* @param          None                                                      *
* @returns        N/A                                                       *
****************************************************************************/
$(function () {
    pollData();
});

function pollData() {
    $(".seriesDescription").html("");
    let utility = new UtilityClass();
    let listOfPromises = [];
    let tempData = {};
    let assignedMatches = [];
    function createPromise(url) {
        let myPromise = new Promise(function (resolve, reject) {
            utility.getRequest(
                url,
                function (data) {
                    resolve(data);
                },
                function (err) {
                    reject("Unable to fetch Live data for" + url);
                }
            );
        });
        return myPromise;
    }

    // Promise to fetch Series info
    listOfPromises.push(createPromise("/api/series"));
    // Promise to fetch Matches info
    listOfPromises.push(createPromise("/api/series/matches"));
     // Promise to fetch Unassigned Matches info
    listOfPromises.push(createPromise("/api/series/unassigned"));   
    
    Promise.all(listOfPromises).then(function (values) {
        let seriesData = values[0];
        let matchData = values[1];
        let unassignedMatchData = values[2];
        let objSeries = {};
        seriesData.map(function (sD) {
            tempData[sD.SeriesId] = {
                "SeriesName": sD.SeriesName,
                "Type": "ODI",
                "Matches": [],
                "SeriesId": sD.SeriesId
            };
        });
        matchData.map(function (mD) {
            assignedMatches.push(mD.unique_id);
            tempData[mD.SeriesId].Matches.push({
                "Date": formatDate(mD.dateTimeGMT),
                "Team1": mD["team-1"],
                "Team2": mD["team-2"],
                "unique_id" : mD.unique_id
            });
        });
        
        processData(Object.values(tempData));
        displayUnassignedMatches(unassignedMatchData, utility, assignedMatches);
    });
}

/****************************************************************************
* @function       processData                                               *
* @brief          Process data once we have it from http call               *
* @param          {Object} data - Data to be rendered                       *
* @returns        N/A                                                       *
****************************************************************************/
function processData(data) {
    let header = "<span id='AddSeries'>+</span>";
    let seriesTitleContent = "<ul>";
    data.map(function (sData) {
        seriesTitleContent += "<li seriesId='" + sData.SeriesId + "'>" + sData.SeriesName + "</li>";
    });
    seriesTitleContent += "</ul>";
    $(".seriesLeftBlock").html(header + seriesTitleContent);

    $("#AddSeries").unbind("click");
    $("#AddSeries").bind("click", function () {
        openPopupAndCreateRecord(data);
    });
    $(".seriesLeftBlock ul li").unbind("click");
    $(".seriesLeftBlock ul li").bind("click", function () {
        $(".seriesLeftBlock ul li").removeClass("selected");
        $(this).addClass("selected");
        let selectedSeries = this.innerText;
        localStorage.SeriesId = this.getAttribute("seriesId");
        data.map(function (mData) {
            if (mData.SeriesName == selectedSeries) {
                displayInRightContainer(mData);
            }
        });
    });
}

/****************************************************************************
* @function       displayInRightContainer                                   *
* @brief          Display matches in selected series                        *
* @param          {Object} seriesDesc                                       *
* @returns        N/A                                                       *
****************************************************************************/
function displayInRightContainer(seriesDesc) {
    let header = "<h3>" + seriesDesc.SeriesName + "</h3>"
    let seriesMatchesContent = "<table><tr><th>Date</th><th>Team 1</th><th>Team 2</th></tr>";
    if (seriesDesc.Matches.length) {
        seriesDesc.Matches.map(function (matches) {
            seriesMatchesContent += "<tr>";
            seriesMatchesContent += "<td>" + matches.Date + "</td>";
            seriesMatchesContent += "<td><img class='allCountries " + matches.Team1.replace(/ /gi, "") + "'/><span>" + matches.Team1 + "</span></td>";
            seriesMatchesContent += "<td><img class='allCountries " + matches.Team2.replace(/ /gi, "") + "'/><span>" + matches.Team2 + "</span></td>";
            seriesMatchesContent += "<td><button class='removeBtn' onclick=\"removeMatchFromSeries('" + matches.unique_id + "')\">Remove</button></td>";
            seriesMatchesContent += "</tr>";
        });
    }
    seriesMatchesContent += "</table>";
    $(".seriesDescription").html(header + seriesMatchesContent);
}

/****************************************************************************
* @function       openPopupAndCreateRecord                                  *
* @brief          Display popup                                             *
* @param          {Object} seriesDesc                                       *
* @returns        N/A                                                       *
****************************************************************************/
function openPopupAndCreateRecord(data) {
    $("#inputPopup").show();
    $(".close").unbind("click");
    $("#addSeries").unbind("click");
    $("#addSeries").bind("click", function () {
        let utility = new UtilityClass();
        let seriesName = $("#serName").val();
        // Clear all data
        $("#serName").val("");
        utility.postRequest(
            "/api/series/" + seriesName,
            function (data) {
                $("#inputPopup").hide();
                pollData();
            },
            function (err) {
                $("#inputPopup").hide();
                alert("Unable to add series");
            }
        );
    });
    $(".close").bind("click", function () {
        $("#inputPopup").hide();
    });
}

/****************************************************************************
* @function       displayUnassignedMatches                                  *
* @brief          Display unassigned matches                                *
* @param          {Object} unAssignedMatches                                *
* @param          {Object} assignedMatches - Array of assigned matches      *
* @returns        N/A                                                       *
****************************************************************************/
function displayUnassignedMatches(unAssignedMatches, utility, assignedMatches) {
    let header = "<h3>UnAssigned Matches</h3>"
    let unAssignedMatchesContent = "<table><tr><th>Date</th><th>Team 1</th><th>Team 2</th><th>Add to this Series</th></tr>";
    let tempObj = {};
    unAssignedMatches.map(function (matches) {
        if( matches["team-1"] && matches["team-2"] &&
            (matches["team-2"] != "Unknown") &&
            (matches["team-1"] != "Unknown" ) &&
            (assignedMatches.indexOf(matches.unique_id) == -1) ) {
            unAssignedMatchesContent += "<tr>";
            unAssignedMatchesContent += "<td>" + formatDate(matches.dateTimeGMT) + "</td>";
            unAssignedMatchesContent += "<td><img class='allCountries " + matches["team-1"].replace(/ /gi, "") + "'/><span>" + matches["team-1"] + "</span></td>";
            unAssignedMatchesContent += "<td><img class='allCountries " + matches["team-2"].replace(/ /gi, "") + "'/><span>" + matches["team-2"] + "</span></td>";
            unAssignedMatchesContent += "<td><button class='addBtn' matchId='" + matches.unique_id + "'>Add</button></td>";
            unAssignedMatchesContent += "</tr>";
            tempObj[matches.unique_id] = matches;          
        }

    });
    unAssignedMatchesContent += "</table>";
    $(".untaggedMatches").html(header + unAssignedMatchesContent);
    $(".addBtn").unbind("click");
    $(".addBtn").bind("click", function() {
        let uniqueId = +this.getAttribute("matchId");
        let currentObj = tempObj[uniqueId];
        currentObj.SeriesId = +localStorage.SeriesId;
        utility.postRequest(
            "/api/series",
            function (data) {
                pollData();
            },
            function (err) {
                alert("Unable to add match to series");
            },
            currentObj
        );
    });
}

/****************************************************************************
* @function       formatDate                                                *
* @brief          Formats date                                              *
* @param          {Object} date                                             *
****************************************************************************/
function formatDate(date) {
    date = new Date(date);
    let months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    return ((months[date.getMonth()]) + ' ' + date.getDate() + ', ' +  date.getFullYear());
}

function removeMatchFromSeries(matchId) {
    let utility = new UtilityClass();
    utility.deleteRequest(
        "/api/series/matches/" + matchId,
        function (data) {
            pollData();
        },
        function (err) {
            alert("Unable to remove match from series");
        }
    );    
}