$(document).ready(function () {
    $("#loadList").click(function (e) {
        e.preventDefault();
        $.ajax({
            type: "GET",
            url: $(this).attr("href"),
            success: function (response) {
                $("#json_container").text(response);
            },
            error: function (error) {
                console.error(error);
            }
        });
    });
});