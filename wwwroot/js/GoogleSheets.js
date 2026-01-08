/* exported gapiLoaded */
/* exported gisLoaded */
/* exported handleAuthClick */
/* exported handleSignoutClick */

//default SS
//
//Range
//Class Data!A2:E26

const CLIENT_ID = '472241564480-hitnimac1ouv1kane9qb52otue9jd3ht.apps.googleusercontent.com';
const API_KEY = 'AIzaSyBeOc7v2MkLFxUKIgHV4IpwT3dwffaEhAQ';

// Discovery doc URL for APIs used by the quickstart
const DISCOVERY_DOC = 'https://sheets.googleapis.com/$discovery/rest?version=v4';

// Authorization scopes required by the API; multiple scopes can be
// included, separated by spaces.
const SCOPES = 'https://www.googleapis.com/auth/spreadsheets.readonly';

let tokenClient;
let gapiInited = false;
let gisInited = false;

document.getElementById('authorize_button').style.visibility = 'hidden';
document.getElementById('import_button').style.visibility = 'hidden';
document.getElementById('signout_button').style.visibility = 'hidden';
document.getElementById('SheetsForm').classList.add("collapse")

/**
 * Callback after api.js is loaded.
 */
function gapiLoaded() {
    gapi.load('client', initializeGapiClient);
}

/**
 * Callback after the API client is loaded. Loads the
 * discovery doc to initialize the API.
 */
async function initializeGapiClient() {
    await gapi.client.init({
        apiKey: API_KEY,
        discoveryDocs: [DISCOVERY_DOC],
    });
    gapiInited = true;
    maybeEnableButtons();
}

/**
 * Callback after Google Identity Services are loaded.
 */
function gisLoaded() {
    tokenClient = google.accounts.oauth2.initTokenClient({
        client_id: CLIENT_ID,
        scope: SCOPES,
        callback: '', // defined later
    });
    gisInited = true;
    maybeEnableButtons();
}

/**
 * Enables user interaction after all libraries are loaded.
 */
function maybeEnableButtons() {
    if (gapiInited && gisInited) {
        document.getElementById('authorize_button').style.visibility = 'visible';
    }
}

/**
 *  Sign in the user upon button click.
 */
function handleAuthClick() {
    tokenClient.callback = async (resp) => {
        if (resp.error !== undefined) {
            throw (resp);
        }
        document.getElementById('signout_button').style.visibility = 'visible';
        document.getElementById('import_button').style.visibility = 'visible';
        document.getElementById('authorize_button').innerText = 'Refresh';
        document.getElementById('SheetsForm').classList.remove("collapse")
        //await GetCards();
        //await listMajors();
    };

    if (gapi.client.getToken() === null) {
        // Prompt the user to select a Google Account and ask for consent to share their data
        // when establishing a new session.
        tokenClient.requestAccessToken({ prompt: 'consent' });
    } else {
        // Skip display of account chooser and consent dialog for an existing session.
        tokenClient.requestAccessToken({ prompt: '' });
    }
}

/**
 *  Sign out the user upon button click.
 */
function handleSignoutClick() {
    const token = gapi.client.getToken();
    if (token !== null) {
        google.accounts.oauth2.revoke(token.access_token);
        gapi.client.setToken('');
        document.getElementById('content').innerText = '';
        document.getElementById('authorize_button').innerText = 'Authorize';
        document.getElementById('signout_button').style.visibility = 'hidden';
        document.getElementById('import_button').style.visibility = 'hidden';
    }
}

/*Allow user to Validate Spreadsheet*/
function CheckSpreadsheet() {
    document.getElementById('SheetsForm').classList.add("collapse")    

    var userSettings = {
        DefaultGoogleSpreadsheet: document.getElementById("SpreadsheetId").value,
        DefaultGoogleRange: document.getElementById("SpreadsheetRange").value
    }
    $.ajax({
        url: "../Home/UpdateUserSettings",
        data: userSettings,
        success: function (result) {
            //console.log(result)
        },
        error: function (result) {
            processingState = "Good"            
        }
    })
    GetCards(userSettings.DefaultGoogleSpreadsheet, userSettings.DefaultGoogleRange)

}

async function GetCards(DefaultGoogleSpreadsheet, DefaultGoogleRange) {
    document.getElementById('SampleExtract').classList.remove("collapse");
    document.getElementById('LoadingBar').classList.remove("collapse")
    document.getElementById('SheetsForm').classList.remove("collapse")
    document.getElementById("SpreadsheetId").disabled = true;
    document.getElementById("SpreadsheetRange").disabled = true;

    UpdateProgressBarGoog("0%")

    console.log({ DefaultGoogleSpreadsheet, DefaultGoogleRange })

    let response;
    try {
        // Fetch first 10 files
        response = await gapi.client.sheets.spreadsheets.values.get({
            spreadsheetId: DefaultGoogleSpreadsheet,
            range: DefaultGoogleRange,
        });
    } catch (err) {
        document.getElementById('content').innerText = err.message;
        return;
    }
    UpdateProgressBarGoog("25%")
    const range = response.result;
    if (!range || !range.values || range.values.length == 0) {
        document.getElementById('content').innerText = 'No values found.';
        return;
    }
    UpdateProgressBarGoog("50%")
    // Flatten to string to display
    var htmlString = "<table class='table text-white'><thead><tr>" +
        "<th>Qty</th>" +
        "<th>Set</th>" +
        "<th>Number</th>" +
        "<th>Mark</th>" +
        "<th>Language</th>" +
        "<th>Name</th>" +
        "<th>Type</th>" +
        "<th>Rarity</th>" +
        "<th>Confirmed</th>" +
        "<th>Location</th>" +
        "<th>Type 1</th>" +
        "<th>Type 2</th>" +
        "<th>Type 3</th>" +
        "<th>Note</th>" +
        "</tr></thead><tbody>"

    console.log(range.values)
    UpdateProgressBarGoog("75%")
    for (i = 1; i < range.values.length; i++) {
        var _Qty = range.values[i][0]
        var _Set = range.values[i][1]
        var _Number = range.values[i][2]
        var _Mark = range.values[i][3]
        var _Lang = range.values[i][4]
        var _Name = range.values[i][5]
        var _Type = range.values[i][6]
        var _Rarity = range.values[i][7]
        var _Confirmed = range.values[i][8]
        var _Location = range.values[i][9]
        var _Type1 = range.values[i][10]
        var _Type2 = range.values[i][11]
        var _Type3 = range.values[i][12]
        var _Note = range.values[i][13]

        htmlString += "<tr><td>" +
            (_Qty === undefined ? "" : _Qty) + "</td><td>" +
            (_Set === undefined ? "" : _Set) + "</td><td>" +
            (_Number === undefined ? "" : _Number) + "</td><td>" +
            (_Mark === undefined ? "" : _Mark) + "</td><td>" +
            (_Lang === undefined ? "" : _Lang) + "</td><td>" +
            (_Name === undefined ? "" : _Name) + "</td><td>" +
            (_Type === undefined ? "" : _Type) + "</td><td>" +
            (_Rarity === undefined ? "" : _Rarity) + "</td><td>" +
            (_Confirmed === undefined ? "" : _Confirmed) + "</td><td>" +
            (_Location === undefined ? "" : _Location) + "</td><td>" +
            (_Type1 === undefined ? "" : _Type1) + "</td><td>" +
            (_Type2 === undefined ? "" : _Type2) + "</td><td>" +
            (_Type3 === undefined ? "" : _Type3) + "</td><td>" +
            (_Note === undefined ? "" : _Note) + "</td>" +
            "</tr>"
    }
    htmlString += "</tbody></table>"
    //const output = range.values.reduce(
    //    (str, row) => `${str}${row[0]}, ${row[4]}\n`,
    //    ':\n');
    document.getElementById('content').innerHTML = htmlString;
    //document.getElementById('btnBlock').classList.add("collapse")
    document.getElementById('LoadingBar').classList.add("collapse")
    document.getElementById("SpreadsheetId").disabled = false;
    document.getElementById("SpreadsheetRange").disabled = false;
    UpdateProgressBarGoog("100%")
}

/*Import Cards*/
async function PostCardsImport() {
    document.getElementById('btnBlock').classList.add("collapse")
    document.getElementById('LoadingBar').classList.remove("collapse")
    UpdateProgressBarGoog("0%")    

    var userSettings = {
        DefaultGoogleSpreadsheet: document.getElementById("SpreadsheetId").value,
        DefaultGoogleRange: document.getElementById("SpreadsheetRange").value
    }

    document.getElementById("SpreadsheetId").disabled = true;
    document.getElementById("SpreadsheetRange").disabled = true;

    let response;
    try {
        // Fetch first 10 files
        response = await gapi.client.sheets.spreadsheets.values.get({
            spreadsheetId: userSettings.DefaultGoogleSpreadsheet,
            range: userSettings.DefaultGoogleRange,
        });
    } catch (err) {
        document.getElementById('content').innerText = err.message;
        return;
    }
    const range = response.result;
    if (!range || !range.values || range.values.length == 0) {
        document.getElementById('content').innerText = 'No values found.';
        return;
    }
    UpdateProgressBarGoog("50%")

    Cards = []
    //console.log(range.values)
    for (i = 1; i < range.values.length; i++) {
        Cards[Cards.length] = {
            Qty: range.values[i][0],
            _Set: range.values[i][1],
            _SetNumber: range.values[i][2],
            Mark: range.values[i][3],
            Language: range.values[i][4],
            Name: range.values[i][5],
            Type: range.values[i][6],
            Confirmed: range.values[i][8],
            Location: range.values[i][9],
            Note: range.values[i][13]
        }
    }

    //Cards = Cards.sort((a, b) => a._Set.localeCompare(b._Set) || parseInt(a._SetNumber) - parseInt(b._SetNumber))

    UpdateProgressBarGoog("75%")
    if (false) {
        var dbClear = true
        await $.ajax({
            url: "../Cards/ResetMyInventory",
            success: function (result) {
                console.log(result)
            },
            error: function (result) {
                console.log(result)
                var dbClear = false;
            }
        })
        if (!dbClear) {
            document.getElementById('processing').innerText = "Could not clear the database. Please try again later.";
            document.getElementById('LoadingBar').classList.add("collapse")
            document.getElementById('btnBlock').classList.remove("collapse")
            document.getElementById("SpreadsheetId").disabled = false;
            document.getElementById("SpreadsheetRange").disabled = false;
            return;
        }
    }
    UpdateProgressBarGoog("100%", "Preparation Complete. Beginning upload...")
    UpdateCards(Cards, 20)
    document.getElementById('btnBlock').classList.remove("collapse")
    document.getElementById("SpreadsheetId").disabled = false;
    document.getElementById("SpreadsheetRange").disabled = false;
}

async function UpdateCards(Cards, chunk) {
    //const chunk = 10;
    for (i = 0; i < Cards.length; i += chunk) {
        var processingState = "Good"
        cardChunk = Cards.slice(i, i + chunk)
        await $.ajax({
            url: "../Cards/UpdateCardsFromSheet",
            data: {
                rows: JSON.stringify(cardChunk)
            },
            success: function (result) {
                //console.log(result)
            },
            error: function (result) {
                console.log("Failed processing chunk " + i + " to " + (i + chunk) + ".")
                processingState = "Bad"
                document.getElementById('LoadingBar').classList.add("collapse")
            }
        })

        if (processingState == "Good") {
            var pct = parseInt((i + chunk) / Cards.length * 10000) / 100
            //document.getElementById('processing').innerText = pct + "%";
            pct = Math.min(pct, 100)
            if (pct >= 100) {
                UpdateProgressBarGoog(pct + "%", "Complete!")
            }
            else if (pct > 25) {
                var lastSet = Cards.slice(i + chunk, i + chunk + 1)[0];
                var lastSetVal = "";
                if (lastSet != undefined) {
                    lastSetVal = lastSet._Set
                }
                UpdateProgressBarGoog(pct + "%", "Working on cards from " + lastSetVal)
            } else {
                UpdateProgressBarGoog(pct + "%", "")
            }
        } else {
            break;
        }
    }
}
function UpdateProgressBarGoog(pct, preface = "") {
    document.getElementById("myBar").style.width = pct
    if (preface != "") {
        document.getElementById("myBarProgress").innerText = preface + " " + pct
    } else {
        document.getElementById("myBarProgress").innerText = pct
    }
}