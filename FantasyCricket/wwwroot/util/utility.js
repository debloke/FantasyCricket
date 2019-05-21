let UtilityClass = function() {
    let that = this;
    that.baseURL = "";
    
    /**************************************************
     * @function     getRequest
     * @brief        Make HTTP GET calls
     * @param        {String} url - URL for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @return       N/A
     **************************************************/
    that.getRequest = function(url, successCB, errorCB) {
        makeHTTPCall(
            {url: (that.baseURL + url)}, 
            successCB,
            errorCB);
    };
    
    /**************************************************
     * @function     postRequest
     * @brief        Make HTTP POST calls
     * @param        {String} url - URL for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @return       N/A
     **************************************************/
    that.postRequest = function(url, successCB, errorCB) {
        makeHTTPCall(
            {url: (that.baseURL + url), type: "POST"}, 
            successCB,
            errorCB);
    };
    
    /**************************************************
     * @function     putRequest
     * @brief        Make HTTP PUT calls
     * @param        {String} url - URL for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @return       N/A
     **************************************************/
    that.putRequest = function(url, successCB, errorCB) {
        makeHTTPCall(
            {url: (that.baseURL + url), type: "PUT"}, 
            successCB,
            errorCB);
    };
    
    /////////////////////////
    //  PRIVATE FUNCTIONS  //
    /////////////////////////
    /**************************************************
     * @function     makeHTTPCall
     * @brief        Make HTTP calls
     * @param        {Object} obj - Object for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @return       N/A
     **************************************************/
    function makeHTTPCall(obj, successCB, errorCB) {
        // AJAX call
        $.ajax(obj)
        .done(function( data ) {
            successCB(data);
        }).fail(function(err) {
            errorCB(err);
        });
    };  
};