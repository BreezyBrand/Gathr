// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function runSearch() {
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
                document.getElementById("searchResults").innerHTML = result
            },
            error: function (result) {
                console.log(result)
            }
        })
    } else if (toggleType == "bulk") {
        $.ajax({
            url: "Cards/GetBulk?set_code=" + set_code + "&card_num=" + card_num + "&lang_code=" + lang_code,
            success: function (result) {
                document.getElementById("searchResults").innerHTML = result;
                reformatCardHeads();
            },
            error: function (result) {
                console.log(result)
            }
        })
    } else {
        document.getElementById("searchResults").innerHTML = "<p>Whoops! We're not ready for you to do that yet!</p>"
    }
}

function searchEZ(cards) {
    console.log(cards)
    $.ajax({
        url: "Cards/EZSearch",
        contentType: "application/json",
        data: { raw_cards: JSON.stringify(cards)},
        success: function (result) {
            document.getElementById("searchResults").innerHTML = result
        },
        error: function (result) {
            console.log(result)
        }
    })
}

function toggleSearch(toggleType) {
    document.getElementById("toggleType").value = toggleType;
}

function AddCard(id) {
    console.log(id)

    $.ajax({
        url: "Cards/AddToInventory?card_id=" + id,
        success: function (result) {
            document.getElementById(id).innerHTML = result
        },
        error: function (result) {
            console.log(result)
        }
    })
}

function UpdateInventory(id, cardid) {
    console.log("Updating card " + id)
    mark = document.getElementById(id + "_mark").value;
    loc = document.getElementById(id + "_loc").value;
    conf = document.getElementById(id + "_conf").checked;    
    lan = document.getElementById(id + "_lan").value;
    row_id = 

    $.ajax({
        url: "Cards/UpdateInventory",        
        data: {
            Card_Id: cardid,
            confirmed_date: new Date(),
            Id:id,
            Language:lan,
            Location:loc,
            Mark:mark,
            _confirmed:conf
        },
        success: function (result) {
            document.getElementById(cardid).innerHTML = result
        },
        error: function (result) {
            console.log(result)
        }
    })
}

function DeleteInventory(id, cardid) {
    console.log("Deleting card " + cardid)

    //$.ajax({
    //    url: "Cards/Delete?card_id=" + id,
    //    success: function (result) {
    //        document.getElementById(cardid).innerHTML = result
    //    },
    //    error: function (result) {
    //        console.log(result)
    //    }
    //})
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