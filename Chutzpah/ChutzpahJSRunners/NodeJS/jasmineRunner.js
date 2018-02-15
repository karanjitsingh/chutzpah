var Jasmine = require('jasmine');
var chutzpah = require('./chutzpahRunner.js');


var jasmine = new Jasmine();

var activeTestCase = null,
    fileStartTime = null,
    testStartTime = null,
    suites = [];

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
        ({ type: "FileStart" });
    };


    this.jasmineDone = function () {
        var timetaken = new Date().getTime() - fileStartTime;
        // logCoverage();
        console.log({ type: "FileDone", timetaken: timetaken, passed: passedCount, failed: failedCount });
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
        activeTestCase = newTestCase;
        console.log({ type: "TestStart", testCase: activeTestCase });
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
        console.log({ type: "TestDone", testCase: activeTestCase });


    };

}

jasmine.clearReporters();
jasmine.addReporter(new ChutzpahJasmineReporter());

if (chutzpah.testMode === 'discovery') {

    var oldSpecExec = jasmine.jasmine.Spec.prototype.execute;
    jasmine.jasmine.Spec.prototype.execute = function (onComplete) {

        this.onStart(this);
        this.resultCallback(this.result);
        onComplete();
    };
}

jasmine.execute(chutzpah.testFiles);
