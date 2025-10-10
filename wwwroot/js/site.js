// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function runSearch() {
    const set_code = document.getElementById("searchSet").value;
    const card_num = document.getElementById("searchNum").value;
    const lang_code = document.getElementById("searchLan").value;
    console.log("Searching...")

    $.ajax({
        url: "Cards/CardDetails?set_code=" + set_code + "&card_num=" + card_num + "&lang_code=" + lang_code,
        success: function (result) {
            document.getElementById("searchResults").innerHTML = result
        },
        error: function (result) {
            console.log(result)
        }
    })
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

function UpdateInventory(id) {
    console.log("Updating card " + id)
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