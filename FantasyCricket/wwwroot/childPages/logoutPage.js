"use strict";
var logoutPage = function() {
    this.returnDataForDisplay = function(id) {
        sessionStorage.removeItem("loggedInUser");
        window.location.href = "index.html";
    };
};