var Jasmine = require('jasmine');

var jasmine = new Jasmine();

var activeTestCase = null,
    fileStartTime = null,
    testStartTime = null,
    suites = [];

function wrap(txt) {
    return '#_#' + txt + '#_# ';
}

function writeEvent(eventType, json) {

    // Everytime we get an event update the startTime. We want timeout to happen
    // when were have gone quiet for too long
    switch (eventType) {
        case 'FileStart':
        case 'TestStart':
        case 'TestDone':
        case 'Log':
        case 'Error':
        case 'CoverageObject':
            console.log(wrap(eventType) + json);
            break;

        case 'FileDone':
            console.log(wrap(eventType) + json);
            break;

        default:
            break;
    }
}

function log(obj) {
    writeEvent(obj.type, JSON.stringify(obj));
}


function recordStackTrace(stack) {
    if (stack) {
        // Truncate stack to 5 deep. 
        stack = stack.split('\n').slice(1, 6).join('\n');
    }
    return stack;
}

var ChutzpahJasmineReporter = function (options) {

    var passedCount = 0;
    var failedCount = 0;
    var skippedCount = 0;

    this.jasmineStarted = function () {

        fileStartTime = new Date().getTime();

        // Testing began
        log({ type: "FileStart" });
    };


    this.jasmineDone = function () {
        var timetaken = new Date().getTime() - fileStartTime;
        // logCoverage();
        log({ type: "FileDone", timetaken: timetaken, passed: passedCount, failed: failedCount });
        // window.chutzpah.isTestingFinished = true;
    };


    this.suiteStarted = function (result) {
        suites.push(result);
    };

    this.suiteDone = function (result) {
        suites.pop();
    };

    this.specStarted = function (result) {

        var currentSuiteName = suites.length > 0
            ? suites[suites.length - 1].fullName
            : null;

        testStartTime = new Date().getTime();
        var suiteName = currentSuiteName;
        var specName = result.description;
        var newTestCase = { moduleName: suiteName, testName: specName, testResults: [] };
        // window.chutzpah.testCases.push(newTestCase);
        activeTestCase = newTestCase;
        log({ type: "TestStart", testCase: activeTestCase });
    };

    this.specDone = function (result) {
        if (result.status === "disabled") {
            return;
        }

        if (result.status === "failed") {
            failedCount++;
        }
        else if (result.status === "pending") {
            skippedCount++;
            activeTestCase.skipped = true;
        }
        else {
            passedCount++;
        }

        var timetaken = new Date().getTime() - testStartTime;
        activeTestCase.timetaken = timetaken;



        for (var i = 0; i < result.failedExpectations.length; i++) {
            var expectation = result.failedExpectations[i];

            var testResult = {};
            testResult.passed = false;
            testResult.message = expectation.message;
            testResult.stackTrace = recordStackTrace(expectation.stack);
            activeTestCase.testResults.push(testResult);

        }

        // Log test case when done. This will get picked up by phantom and streamed to chutzpah.
        log({ type: "TestDone", testCase: activeTestCase });


    };

}

jasmine.clearReporters();
jasmine.addReporter(new ChutzpahJasmineReporter());


var testMode = process.argv[3];

if (testMode === 'discovery') {

    var oldSpecExec = jasmine.jasmine.Spec.prototype.execute;
    jasmine.jasmine.Spec.prototype.execute = function (onComplete) {

        this.onStart(this);
        this.resultCallback(this.result);
        onComplete();
    };
}


jasmine.execute([process.argv[2]]);

