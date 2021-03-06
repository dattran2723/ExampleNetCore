﻿const uri = "api/todo";
let todos = null;
var currentPage = 1;
var pageCount = 1;
var searchName = '';
function getCount(data) {
    const el = $("#counter");
    let name = "to-do";
    if (data) {
        if (data > 1) {
            name = "to-dos";
        }
        el.text(data + " " + name);
    } else {
        el.text("No " + name);
    }
}

$(document).ready(function () {
    getData(currentPage);
});



function getData(page, name) {
    searchName = name === undefined ? '' : name;
    $.ajax({
        type: "GET",
        url: uri + "/get?name=" + searchName + "&page=" + page,
        cache: false,
        success: function (data) {
            pageCount = data.pageCount;
            var btn = "<input type='submit' id='previous' value='previous'>";
            for (var i = 0; i < pageCount; i++) {
                btn += "<input type='submit' class='loadPage' value='" + (i + 1) + "'/>";
            }
            btn += "<input type='submit' id='next' value='next'>";
            $('.pagination').html(btn);

            const tBody = $("#todos");
            $(tBody).empty();

            getCount(data.rowCount)

            $.each(data.results, function (key, item) {
                const tr = $("<tr></tr>")
                    .append(
                        $("<td></td>").append(
                            $("<input/>", {
                                type: "checkbox",
                                disabled: true,
                                checked: item.isComplete
                            })
                        )
                    )
                    .append($("<td></td>").text(item.name))
                    .append(
                        $("<td></td>").append(
                            $("<button>Edit</button>").on("click", function () {
                            editItem(item.id);
                            })
                        )
                    )
                    .append(
                        $("<td></td>").append(
                            $("<button>Delete</button>").on("click", function () {
                            deleteItem(item.id);
                            })
                        )
                    );

                tr.appendTo(tBody);
            });
            todos = data.results;
        }
    });
}

function addItem() {
    const item = {
        name: $("#add-name").val(),
        isComplete: false
    };

    $.ajax({
        type: "POST",
        accepts: "application/json",
        url: uri,
        contentType: "application/json",
        data: JSON.stringify(item),
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Something went wrong!");
        },
        success: function (result) {
            getData(currentPage);
            $("#add-name").val("");
        }
    });
}

function deleteItem(id) {
    $.ajax({
        url: uri + "/" + id,
        type: "DELETE",
        success: function (result) {
            getData(currentPage);
        }
    });
}

function editItem(id) {
    $.each(todos, function (key, item) {
        if (item.id === id) {
            $("#edit-name").val(item.name);
            $("#edit-id").val(item.id);
            $("#edit-isComplete")[0].checked = item.isComplete;
        }
    });
    $("#spoiler").css({ display: "block" });
}

$(".my-form").on("submit", function () {
    const item = {
        name: $("#edit-name").val(),
        isComplete: $("#edit-isComplete").is(":checked"),
        id: $("#edit-id").val()
    };

    $.ajax({
        url: uri + "/" + $("#edit-id").val(),
        type: "PUT",
        accepts: "application/json",
        contentType: "application/json",
        data: JSON.stringify(item),
        success: function (result) {
            getData(currentPage);
        }
    });

    closeInput();
    return false;
});

function closeInput() {
    $("#spoiler").css({ display: "none" });
}


$('.pagination').on('click', '.loadPage', function () {
    currentPage = $(this).val();
    getData(currentPage, searchName);
})

$('.pagination').on('click', '#previous', function () {
    if (currentPage > 1) {
        currentPage -= 1;
        getData(currentPage, searchName);
    }
})

$('.pagination').on('click', '#next', function () {
    if (currentPage < pageCount) {
        currentPage += 1;
        getData(currentPage, searchName);
    }
})