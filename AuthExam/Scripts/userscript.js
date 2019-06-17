$(document).ready(function () {
    console.log("application started");
    let cities = [];
    $.ajax({
        type: "GET",
        url: "/User/Cities",
        success: function (response) {
            cities = response;
        },
        error: function (error) {
            console.error(error);
        }
    });

    $("button#addAddress").click(function (e) {
        e.preventDefault();
        console.log("add new address");
        const form = this.closest("form");
        const addrTable = $(form).find("table").first();
        addrTable.append("<tr>"
            + addInputStr('ZipCode', 6, 12)
            + addSelect(cities, 'City')
            + addInputStr('Street', 6, 50)
            + addInputStr('HouseNumber', 1, 12)
            + "<td><button class='btn btn-primary' onclick='$(this).closest(\"tr\").remove();'>Delete</button></td>"
            + "</tr>");
        refreshValidation($(this));
    });

    $("button#deleteAddress").click(function (e) {
        e.preventDefault();
        $(this).closest('tr').remove();
        setRows();
        refreshValidation($(this));
    });

    function refreshValidation(element) {
        const form = $(element).closest("form");
        $(form).removeData("validator")
            .removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(form);
    }

    function setRows() {
        const trs = $("table tbody").find("tr");
        for (let i = 0; i < trs.length; i++) {
            const inputs = $(trs[i]).find("input, select");
            const labels = $(trs[i]).find("span");
            $(inputs).each(function () {
                const name = $(this).attr("name").split(".")[1];
                $(this).attr("name", "Addresses[" + i + "]." + name);
            });
            $(labels).each(function () {
                const name = $(this).attr("data-valmsg-for").split(".")[1];
                $(this).attr("data-valmsg-for", "Addresses[" + i + "]." + name);
            });
        }
    }

    function addInputStr(name, min, max) {
        const addressCount = $("table tbody").find("tr").length++;
        return "<td><input id='Addresses_" + addressCount + "__" + name + "' data-val='true'"
            + " data-val-length='The " + name + " must be at least " + min + " characters long.'"
            + " data-val-length-max='" + max + "' data-val-length-min='" + min + "'"
            + " data-val-required='A következő mező megadása kötelező: " + name + ".'"
            + " type='text' class='form-control' name = 'Addresses[" + addressCount + "]." + name + "' />"
            + "<span class='text-danger field-validation-valid' data-valmsg-for='Addresses[" + addressCount + "]." + name + "' data-valmsg-replace='true'>"
            + "</span ></td> ";
    }

    function addSelect(list, name) {
        const addressCount = $("table tbody").find("tr").length++;
        let select = "<td><select class='form-control' name='Addresses[" + addressCount + "].Selected" + name + "Id'>";
        $(list).each(function (index, item) {
            select += "<option value='" + item.Id + "'>" + item.Name + "</option>";
        });
        select += "</select></td>";
        return select;
    }
});