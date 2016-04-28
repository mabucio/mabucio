$(document).ready(function () {
    $("li").hover(function () {
        $(this).animate({left:"+=15px" }, 500);
        $(this).animate({left:"-=15px" }, 500);
    });
});