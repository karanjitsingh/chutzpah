module.exports = new function () {

    var executionComplete = false;
    var timeOut = parseInt(process.argv[4]) || 5001;;
    var startTime = new Date().getTime();;

    this.testFiles = process.argv[2].split(';');
    this.testMode = process.argv[3] || "execution";

    // Overridden console.log
    var log;
    var err;

    waitFor(timeOut);

    this.completed = function () {
        this.executionComplete = true;
    }

    this.logEvent = function logEvent(obj) {
        writeEvent(obj, JSON.stringify(obj));
    };

    function waitFor(timeOutMillis) {
        var maxtimeOutMillis = timeOutMillis,
            interval;

        function intervalHandler() {
            var now = new Date().getTime();

            if (this.executionComplete || (now - startTime > maxtimeOutMillis)) {
                if (!executionComplete) {
                    process.exit(3); // Timeout
                } else {
                    clearInterval(interval);
                    process.exit(0);
                }
            }
        }

        interval = setInterval(intervalHandler, 100);
    }

    function wrap(txt) {
        return '#_#' + txt + '#_# ';
    }

    function writeEvent(eventObj, json) {

        // Everytime we get an event update the startTime. We want timeout to happen
        // when were have gone quiet for too long
        startTime = new Date().getTime();
        switch (eventObj.type) {
            case 'FileStart':
            case 'TestStart':
            case 'TestDone':
            case 'Log':
            case 'Error':
            case 'CoverageObject':
                log(wrap(eventObj.type) + json);
                break;

            case 'FileDone':
                log(wrap(eventObj.type) + json);
                break;

            default:
                break;
        }
    }

    function captureLogMessage(obj) {
        try {
            if (typeof (obj) !== 'object' || !obj.type) throw "Unknown object";
            writeEvent(obj, JSON.stringify(obj));

        }
        catch (e) {
            // The message was not a test status object so log as message
            rawLog(obj);
        }
    }

    function rawLog(message) {
        var log = { type: 'Log', log: { message: message } };
        writeEvent(log, JSON.stringify(log));
    }

    function onError(err) {
        var error;
        if (typeof (err) === 'object')
            error = { type: 'Error', error: { message: err.message, StackAsString: err.stack } };
        else
            error = { type: 'Error', error: { message: err } };
        writeEvent(error, JSON.stringify(error));
        process.exit(1);
    }

    if (process.argv.length <= 1) {
        console.log('Error: too few arguments');
        process.exit();
    }

    // Override console.log && console.error
    (function () {
        log = console.log;
        err = console.error;
        console.log = captureLogMessage;
        console.error = onError;
    })();

    // Capture all uncaught exceptions and wrap before logging
    process.on('uncaughtException', onError);

}();
