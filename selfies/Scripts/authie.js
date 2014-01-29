$(document).ready(function () {

    $("abbr.timeago").timeago('updateFromDOM');

    $("a.remover").click(function () {
        var currentId = $(this).attr('id');
        $("#snap-" + currentId).fadeOut();

        $.ajax({
            url: "api/thread",
            data: { '': currentId },
        type: 'DELETE',
        success: function (result) {
            // Do something with the result
            }
        });

    });

});
