var lastRequestId = "";

//GET
function runSearch() {
    lastRequestId = makeid();
    var RequestId = lastRequestId

    const set_code = document.getElementById("searchSet").value;
    const card_num = document.getElementById("searchNum").value;
    const lang_code = document.getElementById("searchLan").value;
    const toggleType = document.getElementById("toggleType").value;
    console.log("Searching...")

    if (set_code == "" && card_num == "" && lang_code == "") {
        return;
    }

    if (toggleType == "database") {
        $.ajax({
            url: "Cards/CardDetails?set_code=" + set_code + "&card_num=" + card_num + "&lang_code=" + lang_code,
            success: function (result) {
                if (RequestId == lastRequestId) {
                    document.getElementById("searchResults").innerHTML = result
                    toggleInventory()
                }
            },
            error: function (result) {
                console.log(result)
            }
        })
    }
    else if (toggleType == "bulk") {
        $.ajax({
            url: "Cards/GetBulk?set_code=" + set_code + "&card_num=" + card_num + "&lang_code=" + lang_code,
            success: function (result) {
                if (RequestId == lastRequestId) {
                    document.getElementById("searchResults").innerHTML = result;
                    reformatCardHeads();
                    toggleInventory()
                }
            },
            error: function (result) {
                console.log(result)
            }
        })
    }
    else if (toggleType == "inventory") {
        $.ajax({
            url: "Cards/GetInventory?set_code=" + set_code + "&card_num=" + card_num + "&lang_code=" + lang_code,
            success: function (result) {
                if (RequestId == lastRequestId) {
                    document.getElementById("searchResults").innerHTML = result;
                    reformatCardHeads();
                    toggleInventory()
                }
            },
            error: function (result) {
                console.log(result)
            }
        })
    }
    else {
        document.getElementById("searchResults").innerHTML = "<p>Whoops! We're not ready for you to do that yet!</p>"
    }
}

function searchEZ(cards) {
    lastRequestId = makeid();
    var RequestId = lastRequestId;

    ajaxReq = $.ajax({
        url: "Cards/EZSearch",
        contentType: "application/json",
        data: { raw_cards: JSON.stringify(cards) },
        success: function (result) {
            if (RequestId == lastRequestId) {
                document.getElementById("searchResults").innerHTML = result
                toggleInventory()
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
}

function getTransactions() {
    lastRequestId = makeid();
    var RequestId = lastRequestId;

    const set_code = document.getElementById("searchSet").value;
    const card_num = document.getElementById("searchNum").value;
    const lang_code = document.getElementById("searchLan").value;

    ajaxReq = $.ajax({
        url: "Cards/TransactionLog?set_code=" + set_code + "&card_num=" + card_num + "&lang_code=" + lang_code,
        success: function (result) {
            if (RequestId == lastRequestId) {
                document.getElementById("searchResults").innerHTML = result
                toggleInventory()
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
}
//POST
function AddCard(id) {
    console.log(id)
    lastRequestId = makeid();
    var RequestId = lastRequestId;

    $.ajax({
        url: "Cards/AddToInventory?card_id=" + id,
        success: function (result) {
            if (RequestId == lastRequestId) {
                document.getElementById(id).innerHTML = result
                ajaxReq = null;
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
}

function UpdateInventory(id, cardid) {
    lastRequestId = makeid();
    var RequestId = lastRequestId
    console.log("Updating card " + id)
    mark = document.getElementById(id + "_mark").value;
    loc = document.getElementById(id + "_loc").value;
    conf = document.getElementById(id + "_conf").checked;
    lan = document.getElementById(id + "_lan").value;

    $.ajax({
        url: "Cards/UpdateInventory",
        data: {
            Card_Id: cardid,
            confirmed_date: new Date(),
            Id: id,
            Language: lan,
            Location: loc,
            Mark: mark,
            _confirmed: conf
        },
        success: function (result) {
            if (RequestId == lastRequestId) {
                document.getElementById(cardid).innerHTML = result
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
}

function DeleteInventory(id, cardid) {
    lastRequestId = makeid();
    var RequestId = lastRequestId;

    console.log("Deleting card " + id)

    mark = document.getElementById(id + "_mark").value;
    loc = document.getElementById(id + "_loc").value;
    conf = document.getElementById(id + "_conf").checked;
    lan = document.getElementById(id + "_lan").value;

    $.ajax({
        url: "Cards/DeleteFromInventory",
        data: {
            Card_Id: cardid,
            confirmed_date: new Date(),
            Id: id,
            Language: lan,
            Location: loc,
            Mark: mark,
            _confirmed: conf
        },
        success: function (result) {
            if (RequestId == lastRequestId) {
                document.getElementById(cardid).innerHTML = result
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
}


//Display
function toggleSearch(toggleType) {
    document.getElementById("toggleType").value = toggleType;
    document.getElementById("searchResults").innerHTML = "";
    runSearch();
}

function toggleMassEntry() {
    var massEntry = document.getElementById("MassEntry");
    massEntry.classList.toggle("collapse")
}

function toggleInventory() {
    var invCheck = document.getElementById("toggleInv").checked
    var all_Results = document.getElementsByClassName("invCard")
    console.log("Toggle for Inventory set to " + invCheck + ".")
    console.log("Located  " + all_Results.length + " cards in search.")
    if (invCheck) {
        for (i = 0; i < all_Results.length; i++) {
            var thisEle = all_Results[i]
            if (thisEle.classList.contains("InInventory")) {
                console.log("Card " + i + " in inventory")
                thisEle.classList.remove("collapse")
            } else {
                console.log("Card " + i + " not in inventory")
                thisEle.classList.add("collapse")
            }
        }
    } else {
        for (i = 0; i < all_Results.length; i++) {
            var thisEle = all_Results[i]
            thisEle.classList.remove("collapse")
        }
    }
}


//Text Parsing
function parseBulkSearch() {
    var raw_val = document.getElementById("bulkEntry").value;

    var raw_cards = raw_val.split("\n")
    var ez_cards = [];
    for (i = 0; i < raw_cards.length; i++) {
        try {
            console.log(raw_cards[i]);
            ez_card = raw_cards[i].split(":")
            ez_cards[i] = {
                SetCode: ez_card[0],
                CardNum: ez_card[1]
            };
        } catch (e) {

        }
    }
    searchEZ(ez_cards);
}

//Table Functions
function SortTable(field) {
    //<i class="bi bi-arrow-up"></i>
    //<i class="bi bi-arrow-down"></i>
    document.getElementById("sortName").innerHTML = "";
    document.getElementById("sortSet").innerHTML = "";
    document.getElementById("sortCN").innerHTML = "";
    document.getElementById("sortMark").innerHTML = "";
    document.getElementById("sortLang").innerHTML = "";
    document.getElementById("sortConfirmed").innerHTML = "";
    document.getElementById("sortColors").innerHTML = "";
    document.getElementById("sortType").innerHTML = "";

    var table = document.getElementById("InvTable").getElementsByTagName("tbody")



}

//Stupid Formatting Code
function reformatCardHeads() {
    var headers = document.getElementsByClassName("card-header")
    var max_height = 0;
    for (i = 0; i < headers.length; i++) {
        if (headers[i].offsetHeight > max_height) {
            max_height = headers[i].offsetHeight
        }
    }
    console.log("max header height is " + max_height)
    for (i = 0; i < headers.length; i++) {
        headers[i].style["min-height"] = max_height + "px"
        headers[i].style["height"] = max_height + "px"
    }
}

//Request validation
function makeid() {
    var result = '';
    var length = 25;
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }    
    return result;
}