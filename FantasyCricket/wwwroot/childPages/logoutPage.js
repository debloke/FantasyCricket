"use strict";
var logoutPage = function() {
    this.returnDataForDisplay = function(id) {
        localStorage.removeItem("loggedInUser");
        window.location.href = "index.html";
    };
};