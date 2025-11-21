var lastRequestId = "";

function UpdateSearchStats(updateSkip, max) {
    console.log("Updating search stats")
    document.getElementById("LoadedRows").value = updateSkip;
    var loads = document.getElementsByClassName("InvCard")
    var count = 0;
    for (i = 0; i < loads.length; i++) {
        if (!loads[i].classList.contains("collapse")) {
            count++
        }
    }
    document.getElementById("SearchResultsText").innerHTML = "Showing " + count + " of " + max
}

function getSearchTerms() {

    const searchName = document.getElementById("searchName").value;
    const set_code = document.getElementById("searchSet").value;
    const card_num = document.getElementById("searchNum").value;
    const lang_code = document.getElementById("searchLan").value;
    const tags = document.getElementById("searchTag").value;
    const colors = document.getElementById("searchColors").value;
    const oracle = document.getElementById("searchOracle").value;
    var location = document.getElementById("searchLoc").value;
    const typeLines = document.getElementById("searchTypeLine").value;
    const toggleType = document.getElementById("toggleType").value;

    if (location == "") {
        location = "Any"
    }

    var searchData = {
        name: searchName,
        set_code: set_code,
        card_num: card_num,
        lang_code: lang_code,
        tags: tags,
        color: colors,
        location: location,
        type: typeLines,
        subtype: typeLines,
        oracle: oracle,
        skip: 0,
        isValid: true,
        toggleType: toggleType
    }

    const ck1 = (searchName != "" || set_code != "" || card_num != "" || lang_code != "" || tags != "" || typeLines != "")
    var ck2 = false;

    if (toggleType == "location") {
        ck2 = true
    } else {
        ck2 = (location != "Any")
    }

    console.log(ck1)
    console.log(ck2)

    const valid = (ck1 || ck2)
    console.log(valid)
    searchData["isValid"] = valid;

    console.log(searchData)
    return searchData;
}

//GET
function runSearch() {
    lastRequestId = makeid();
    var RequestId = lastRequestId;
    var searchData = getSearchTerms()

    if (!searchData["isValid"]) {
        document.getElementById("searchResults").innerHTML = "";
        document.getElementById("searchResults").classList.remove("collapse");
        document.getElementById("LoadingShuffle").classList.add("collapse");
        return;
    }

    if (searchData["toggleType"] == "database") {
        console.log("Searching full database...")
        $.ajax({
            url: "Cards/CardDetails",
            data: searchData,
            success: function (result) {
                if (RequestId == lastRequestId) {
                    document.getElementById("searchResults").innerHTML = result;
                    document.getElementById("searchResults").classList.remove("collapse");
                    document.getElementById("LoadingShuffle").classList.add("collapse");
                    toggleInventory();
                    EndRequest()
                }
            },
            error: function (result) {
                console.log(result)
            }
        })
    }
    else if (searchData["toggleType"] == "bulk") {
        console.log("Searching for bulk entry...")
        $.ajax({
            url: "Cards/GetBulk",
            data: searchData,
            success: function (result) {
                if (RequestId == lastRequestId) {
                    document.getElementById("searchResults").innerHTML = result;
                    document.getElementById("searchResults").classList.remove("collapse");
                    document.getElementById("LoadingShuffle").classList.add("collapse");
                    toggleInventory();
                    EndRequest()
                }
            },
            error: function (result) {
                console.log(result);
            }
        })
    }
    else if (searchData["toggleType"] == "location") {
        console.log("Searching by locations...")
        $.ajax({
            url: "Cards/GetLocations",
            data: searchData,
            success: function (result) {
                if (RequestId == lastRequestId) {
                    document.getElementById("locationFilter").innerHTML = result;
                    document.getElementById("searchResults").classList.remove("collapse");
                    document.getElementById("LoadingShuffle").classList.add("collapse");
                    toggleInventory();
                    EndRequest()
                }
            },
            error: function (result) {
                console.log(result);
            }
        })
    }
    else if (searchData["toggleType"] == "inventory") {
        console.log("Searching inventory...")
        $.ajax({
            url: "Cards/GetInventory",
            data: searchData,
            success: function (result) {
                console.log(result.length)
                if (RequestId == lastRequestId) {

                    document.getElementById("searchResults").innerHTML = result;
                    document.getElementById("searchResults").classList.remove("collapse");
                    document.getElementById("LoadingShuffle").classList.add("collapse");

                    try {
                        if (document.getElementById("InvTableendResults")) {
                            document.getElementById("LoadMoreFooterRow").innerHTML = "";
                        }
                    } catch (e) {

                    } finally {
                        toggleInventory();
                        EndRequest()
                    }
                }
            },
            error: function (result) {
                console.log(result);
            }
        })
    }
    else {
        document.getElementById("searchResults").innerHTML = "<p>Whoops! We're not ready for you to do that yet!</p>";
        document.getElementById("searchResults").classList.remove("collapse");
        document.getElementById("LoadingShuffle").classList.add("collapse");
    }
}
function runMiniSearch(source) {
    lastRequestId = makeid();
    var RequestId = lastRequestId;
    document.getElementById("searchResults").classList.remove("collapse");
    const set_code = document.getElementById("searchSet").value;
    const card_num = document.getElementById("searchNum").value;
    const lang_code = document.getElementById("searchLan").value;
    const tags = document.getElementById("searchTag").value;
    const toggleType = document.getElementById("toggleType").value;
    console.log("Searching...")

    var count = document.getElementById("LoadedRows").value;

    if (set_code == "" && card_num == "" && lang_code == "" && tags == "") {
        document.getElementById("searchResults").classList.remove("collapse");
        document.getElementById("LoadingShuffle").classList.add("collapse");
        return;
    }

    if (toggleType == "database") {
        if (document.getElementById("miniSearchBlock")) {
            document.getElementById("miniSearchBlock").remove();
        }
        $.ajax({
            url: "Cards/CardDetails?set_code=" + set_code + "&card_num=" + card_num + "&lang_code=" + lang_code + "&tags=" + tags + "&skip=" + count,
            success: function (result) {
                if (RequestId == lastRequestId) {
                    //YOU ARE HERE
                    //YOU NEED TO UPDATE THE CALLBACK HERE
                    //CREATE A NEW FUNCTION TO RETURN THE SEARCH DATA FOR THE AJAX REQUEST
                    document.getElementById("searchResults").innerHTML += result;
                    document.getElementById("searchResults").classList.remove("collapse");
                    document.getElementById("LoadingShuffle").classList.add("collapse");
                    document.getElementById("currentlyProcessing").value = "false";
                    toggleInventory();
                    EndRequest()
                }
            },
            error: function (result) {
                console.log(result)
            }
        })
    }

    if (source == "inventory") {
        //document.getElementById("LoadMoreFooterRow")
        $.ajax({
            url: "Cards/GetInventory?set_code=" + set_code + "&card_num=" + card_num + "&lang_code=" + lang_code + "&tags=" + tags + "&skip=" + count,
            success: function (result) {
                if (RequestId == lastRequestId) {
                    document.getElementById("InvTableBody").innerHTML += result;
                    document.getElementById("searchResults").classList.remove("collapse");
                    document.getElementById("LoadingShuffle").classList.add("collapse");
                    toggleInventory();
                    EndRequest()
                }
            },
            error: function (result) {
                console.log(result)
            }
        })
    }

}
function ExpandName(name) {
    lastRequestId = makeid();
    var RequestId = lastRequestId;
    const toggleType = document.getElementById("toggleType").value;
    var searchData = getSearchTerms()
    console.log("Searching...")

    searchData["name"] = name;
    searchData["location"] = "Any";    
    searchData["limit"] = false;    

    $.ajax({
        url: "Cards/AllCardsByName/",
        data: searchData,
        success: function (result) {
            if (RequestId == lastRequestId) {
                document.getElementById("drillDownBody").innerHTML = result;
                document.getElementById("searchResults").classList.add("collapse");
                document.getElementById("drillDown").classList.remove("collapse");
                document.getElementById("LoadingShuffle").classList.add("collapse");
                toggleInventory();
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
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
                document.getElementById("searchResults").classList.remove("collapse")
                document.getElementById("LoadingShuffle").classList.add("collapse")
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
                document.getElementById("searchResults").classList.remove("collapse")
                document.getElementById("LoadingShuffle").classList.add("collapse")
                toggleInventory()
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
}
function ToggleLocationId(src, id, name, type) {
    var dir = "";
    if (src.classList.contains("btn-secondary")) {
        //Add cards
        dir = "add";
    } else {
        //Remove cards
        dir = "rem";
    }

    src.classList.toggle("btn-secondary")
    src.classList.toggle("btn-primary")
    src.children[1].classList.toggle("bg-primary")
    src.children[1].classList.toggle("bg-secondary")

    var searchData = getSearchTerms()
    searchData["LocationId"] = id;
    if (searchData["toggleType"] == "location") {
        if (dir == "add") {
            $.ajax({
                url: "Cards/GetCardsByLocation",
                data: searchData,
                success: function (result) {
                    var newHtml = "<div id='searchQueryLocation" +
                        id +
                        "'><h4 class='w-100'>" +
                        type + " - " + name +
                        "</h4>" +
                        result +
                        "</div>";
                    document.getElementById("searchResults").innerHTML += newHtml;
                    document.getElementById("searchResults").classList.remove("collapse");
                    document.getElementById("LoadingShuffle").classList.add("collapse");
                    toggleInventory();
                    EndRequest();
                },
                error: function (result) {
                    console.log(result)
                }
            })
        }
        else {
            document.getElementById("searchQueryLocation" + id).remove();
        }
    } else {
        var locationNames = document.getElementById("searchLoc").value.split(",");
        document.getElementById("searchLoc").value = ""
        if (dir == "add") {
            locationNames[locationNames.length] = type + " - " + name;
        }
        else {
            locationNames.splice(locationNames.indexOf(type + " - " + name), 1);
        }
        document.getElementById("searchLoc").value = cleanJoin(locationNames.filter(onlyUnique))
        runSearch()
    }
}
//POST
function AddCard(id) {
    console.log(id)
    lastRequestId = makeid();
    var RequestId = lastRequestId;
    document.getElementById("searchResults").classList.remove("collapse")
    document.getElementById("LoadingShuffle").classList.add("collapse")

    $.ajax({
        url: "Cards/AddToInventory?card_id=" + id,
        success: function (result) {
            if (RequestId == lastRequestId) {
                if (document.getElementById("toggleType").value == "inventory") {
                    document.getElementById("toggleType").value = "inventory"
                    runSearch();
                } else {
                    document.getElementById(id).innerHTML = result
                }
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
}

function UpdateInventory(id, cardid, rowID) {
    lastRequestId = makeid();
    var RequestId = lastRequestId
    console.log("Updating card " + id + "(" + rowID + ")")
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
            if (document.getElementById("toggleType").value == "inventory") {
                document.getElementById("toggleType").value = "inventory"
                runSearch();
            } else {
                document.getElementById(rowID).innerHTML = result
                EndRequest();
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
}

function CloneInventory(id, cardid, row_id) {
    lastRequestId = makeid();
    var RequestId = lastRequestId
    console.log("Updating card " + id)
    mark = document.getElementById(id + "_mark").value;
    loc = document.getElementById(id + "_loc").value;
    conf = document.getElementById(id + "_conf").checked;
    lan = document.getElementById(id + "_lan").value;

    $.ajax({
        url: "Cards/CloneInventory",
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
                if (document.getElementById("toggleType").value == "inventory") {
                    document.getElementById("toggleType").value = "inventory"
                    runSearch();
                } else {
                    document.getElementById(rowID).innerHTML = result
                    EndRequest();
                }
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
}

function DeleteInventory(id, cardid, rowID) {
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
                if (document.getElementById("toggleType").value == "inventory") {
                    document.getElementById("toggleType").value = "inventory"
                    runSearch();
                } else {
                    document.getElementById(rowID).innerHTML = result
                    toggleInventory()
                    EndRequest();
                }
            }
        },
        error: function (result) {
            console.log(result)
        }
    })
}

function updateBulk(sInput, cardID, mark) {
    console.log(cardID + "|" + mark + " new value: " + sInput.value)
    var updateData = {
        CardId,
        mark,
        newCount: sInput.value
    }
    $.ajax({
        url: "",
        data: updateData,
        success: function (result) {
            sInput.value = result
        }
    })


}

function CreateNewLocType(src) {
    src.disabled = true;
    var name = document.getElementById("NewLocTypeInput").value

    var SubmitData = {
        Name: name,
        Type: name,
        Tier: 1,
        Count: 0
    }
    document.getElementById("NewLocTypeInput").value = "";
    AddNewLocation(SubmitData)
}
function CreateNewLocation(src, type, inpId) {
    src.disabled = true;
    var SubmitData = {
        Name: document.getElementById(inpId).value,
        Type: type,
        Tier: 2,
        Count: 0
    }
    document.getElementById(inpId).value = "";
    AddNewLocation(SubmitData)
}

function AddNewLocation(SubmitData) {
    console.log(SubmitData);

    $.ajax({
        url: "Cards/AddNewLocation",
        data: SubmitData,
        success: function (result) {
            document.getElementById("locationFilter").innerHTML = result;
        }
    })
}
//Display
function toggleSearch(toggleType) {
    document.getElementById("toggleType").value = toggleType;
    if (toggleType == "inventory") {
        document.getElementById("toggleInv").checked = true;
    }
    document.getElementById("searchResults").innerHTML = "";
    document.getElementById("searchResults").classList.add("collapse")
    document.getElementById("LoadingShuffle").classList.remove("collapse")
    runSearch();
}

function toggleAdvancedOptions(btn) {
    var advOpts = document.getElementsByClassName("AdvSearch")
    btn.classList.toggle("btn-secondary")
    btn.classList.toggle("btn-primary")

    for (i = 0; i < advOpts.length; i++) {
        advOpts[i].classList.toggle("collapse")
    }
}

function toggleLocations(btn) {
    btn.classList.toggle("btn-secondary")
    btn.classList.toggle("btn-primary")
    document.getElementById("locationFilter").classList.toggle("collapse");
}

function toggleMassEntry(btn) {
    btn.classList.toggle("btn-secondary")
    btn.classList.toggle("btn-primary")
    document.getElementById("MassEntry").classList.toggle("collapse");
    document.getElementById("basicSearch").classList.toggle("collapse")
}

function toggleInventory() {
    var invCheck = document.getElementById("toggleInv").checked
    var notInvCheck = document.getElementById("toggleNotInv").checked
    var all_Results = document.getElementsByClassName("invCard")

    for (i = 0; i < all_Results.length; i++) {
        var thisEle = all_Results[i]
        thisEle.classList.add("collapse")

        if (thisEle.classList.contains("InInventory") && invCheck) {
            thisEle.classList.remove("collapse")
        } else if (!thisEle.classList.contains("InInventory") && notInvCheck) {
            thisEle.classList.remove("collapse")
        } else {
            thisEle.classList.add("collapse")
        }
    }
}

function toggleColor(e, color) {
    //event.stopPropagation()
    src = document.getElementById("color" + color)
    src.checked = !src.checked;

    var cSearch = document.getElementById("searchColors");
    var colors = [];
    var new_val = "";
    var eles = document.getElementsByClassName("cSelect");
    var noneChecked = true;
    for (i = 0; i < eles.length; i++) {
        if (eles[i].children[1].checked) {
            colors[colors.length] = eles[i].children[1].value
            noneChecked = false;
            eles[i].children[0].innerHTML = "<i class='bi bi-check2 text-success'></i>"
        } else {
            eles[i].children[0].innerHTML = '<i class="bi bi-ban text-danger"></i>'
        }
    }
    if (noneChecked) {
        for (i = 0; i < eles.length; i++) {
            eles[i].children[0].innerHTML = '<i class="bi bi-dash text-secondary"></i>'
        }
    }


    console.log(colors)
    new_val = cleanJoin(colors.filter(onlyUnique));
    cSearch.value = new_val;
    runSearch();
}

function ToggleDropdownLocation(src, type) {
    var val = src.children[2].innerHTML;
    if (val == "false") {
        src.children[2].innerHTML = "true";
    } else {
        src.children[2].innerHTML = "false";
    }

    var locs = [];

    var eles = document.getElementsByClassName("lSelect");
    var noneChecked = true;
    for (i = 0; i < eles.length; i++) {
        if (eles[i].children[2].innerHTML == "true") {
            noneChecked = false;
            locs[locs.length] = type + " - " + eles[i].children[1].innerHTML
            eles[i].children[0].innerHTML = "<i class='bi bi-check2 text-success'></i>"
        } else {
            eles[i].children[0].innerHTML = '<i class="bi bi-ban text-danger"></i>'
        }
    }
    if (noneChecked) {
        for (i = 0; i < eles.length; i++) {
            eles[i].children[0].innerHTML = '<i class="bi bi-dash text-secondary"></i>'
        }
    }

    console.log(locs)
    var new_val = cleanJoin(locs.filter(onlyUnique));
    document.getElementById("searchLoc").value = new_val
    runSearch()
}

function ToggleDropdownGroupDisplay(src, name) {
    eles = document.getElementsByClassName("lSelect")
    var eleDir = false;
    for (i = 0; i < eles.length; i++) {
        if (eles[i].children[3].innerHTML == name) {
            eles[i].classList.toggle("collapse")
            eleDir = eles[i].classList.contains("collapse")
        }
    }

    if (eleDir) {
        src.children[0].children[1].innerHTML = '<i class="bi bi-chevron-down"></i>'
    } else {
        src.children[0].children[1].innerHTML = '<i class="bi bi-chevron-double-down"></i>'
    }

}
function returnDrilDown() {
    document.getElementById("searchResults").classList.remove("collapse");
    document.getElementById("drillDown").classList.add("collapse");
    document.getElementById("LoadingShuffle").classList.add("collapse")
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
function onlyUnique(value, index, array) {
    return array.indexOf(value) === index;
}
function cleanJoin(array) {
    var val = "";
    for (i = 0; i < array.length; i++) {
        if (array[i] != "" && array[i].length > 0) {
            val += array[i] + ","
        }
    }
    console.log(val.slice(0, -1));
    return val.slice(0, -1);
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

function ToggleOracleText(p) {
    p.children[1].classList.toggle("collapse")
    if (p.children[1].classList.contains("collapse")) {
        p.children[2].innerHTML = "<br>(Show)"
    } else {
        p.children[2].innerHTML = "<br>(Hide)"
    }
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

    var headers2 = document.getElementsByClassName("card-body");
    var max_height2 = 0;
    for (i = 0; i < headers2.length; i++) {
        if (headers2[i].offsetHeight > max_height2) {
            max_height2 = headers2[i].offsetHeight
        }
    }
    console.log("max header height is " + max_height2)
    for (i = 0; i < headers2.length; i++) {
        headers2[i].style["min-height"] = max_height2 + "px"
        headers2[i].style["height"] = max_height2 + "px"
    }
}

function updateMarkings(id) {
    //name: Marks_@mark_id
    //id: none_@mark_id
    var none = document.getElementById("none_" + id + "_mark");
    var foil = document.getElementById("foil_" + id + "_mark");
    var etch = document.getElementById("etched_" + id + "_mark");
    var promo = document.getElementById("promo_" + id + "_mark");
    var plist = document.getElementById("list_" + id + "_mark");
    var pInput = document.getElementById(id + "_mark");
    sleep(500)

    if (!foil.checked && !etch.checked && !promo.checked && !plist.checked) {
        none.checked = true
        pInput.value = '-';
        return;
    }
    else {
        none.checked = false
    }

    pInput.value = "";
    if (foil.checked) {
        pInput.value = "f"
    }
    if (etch.checked) {
        pInput.value += "-etched"
    }
    if (promo.checked) {
        pInput.value += "-pp"
    }
    if (plist.checked) {
        pInput.value += " list"
    }

    pInput.value = pInput.value.trim()
    if (pInput.value == "") {
        pInput.value = "-"
    }
}

function ResetMark(id) {
    var none = document.getElementById("none_" + id + "_mark");
    var foil = document.getElementById("foil_" + id + "_mark");
    var etch = document.getElementById("etched_" + id + "_mark");
    var promo = document.getElementById("promo_" + id + "_mark");
    var plist = document.getElementById("list_" + id + "_mark");
    var pInput = document.getElementById(id + "_mark");
    if (none.checked) {
        foil.checked = false;
        etch.checked = false;
        promo.checked = false;
        plist.checked = false;
    }
}
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

//Request validation
function makeid() {
    document.getElementById("searchResults").classList.add("collapse")
    document.getElementById("LoadingShuffle").classList.remove("collapse")
    //document.getElementById("SearchResultReportP").innerHTML = "Searching..."
    document.getElementById("currentlyProcessing").value = true;

    var result = '';
    var length = 25;
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
}

function EndRequest() {
    var count = document.getElementsByClassName("invCard").length
    document.getElementById("LoadedRows").value = count;
    reformatCardHeads()
    document.getElementById("currentlyProcessing").value = false;
}

var imgModal = document.getElementById('imageModal')
imgModal.addEventListener('shown.bs.modal', function (e) {
})

function imgUpdate(e) {
    console.log(e)
    document.getElementById("HighlightImage").src = e.src;
    document.getElementById("imageModalLabel").innerHTML = e.title
}

window.addEventListener('scroll', () => {
    const scrollPosition = window.scrollY;
    const visibleHeight = window.innerHeight;
    const totalHeight = document.documentElement.scrollHeight;

    // Check if the user is at the very bottom (within a small tolerance)
    if (scrollPosition + visibleHeight >= totalHeight - 1) {
        console.log("Scrolled to the bottom of the page!");
        var running = document.getElementById("currentlyProcessing").value
        if (running == "false") {
            runMiniSearch('Card Details')
        }

        // You can trigger your desired action here, e.g., load more content
    }
});