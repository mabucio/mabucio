$(document).ready(function () {
   
    $("#startDate").val(startDate);
    $("#endDate").val(endDate);
    var regEx = new RegExp(/^\d{4}(-|\0*)\d{2}(-|\0*)\d{2}$/);
    var startDateOk = true;
    var endDateOk = true;
    $("#startDate").css("background-color", "#e6ffe6");
    $("#endDate").css("background-color", "#e6ffe6");
    $("#submitDate").prop('disabled', false);

    $("#startDate").bind('input', function () {
        if (regEx.test($(this).val()) || $(this).val() === '') {
            $(this).css("background-color", "#e6ffe6");
            startDateOk = true;
            if (startDateOk && endDateOk)
                $("#submitDate").prop('disabled', false);
        }
        else {
            $(this).css("background-color", "#ff8080");
            startDateOk = false;
            $("#submitDate").prop('disabled', true);
        }

    });

    $("#endDate").bind('input', function () {
        if (regEx.test($(this).val()) || $(this).val() === '') {
            $(this).css("background-color", "#e6ffe6");
            endDateOk = true;
            if (startDateOk && endDateOk)
                $("#submitDate").prop('disabled', false);
        }
        else {
            $(this).css("background-color", "#ff8080");
            endDateOk = false;
            $("#submitDate").prop('disabled', true);
        }
    });
});