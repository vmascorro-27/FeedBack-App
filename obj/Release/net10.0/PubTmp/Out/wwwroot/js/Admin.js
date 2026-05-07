function GetFeedBacksErrorMarkup() {
    return `
            <div class="text-center py-5">
                <i class="fa-solid fa-triangle-exclamation fa-2x text-danger"></i>
                <p class="mt-3 mb-0 fw-semibold">The surveys could not be loaded.</p>
                <button type="button" class="btn btn_feedback mt-2" onclick="GetFeedBacks()">Retry</button>
            </div>`;
}


function GetFeedBacks() {
    var token = $('input[name="__RequestVerificationToken"]').val();

    var date_start = $("#date_start").val();
    var date_end = $("#date_end").val();
    var client_remoties = $("#select_client_remoties").val();

    $.ajax({
        url: '../ADMIN/GetFeedBacks',
        type: 'POST',
        loaderTitle: 'Loading feedback...',
        loaderText: 'We are retrieving the requested surveys..',
        headers: {
            'RequestVerificationToken': token
        },
        data: {
            date_start: date_start,
            date_end: date_end,
            client_remoties: client_remoties
        },
        success: function (result) {
            $('#div_feedback').html(result);
            $('#datatable_feedbacks').DataTable({
                responsive: true,
                paging: true,
                pageLength: 10,
                lengthMenu: [5, 10, 25, 50, 100],
                language: {
                    search: "Search:",
                    lengthMenu: "Show _MENU_ entries",
                    info: "Showing _START_ to _END_ of _TOTAL_ entries",
                    infoEmpty: "Showing 0 to 0 of 0 entries",
                    infoFiltered: "(filtered from _MAX_ total entries)",
                    zeroRecords: "No matching records found",
                    emptyTable: "No data available in table",
                    paginate: {
                        first: "First",
                        previous: "Previous",
                        next: "Next",
                        last: "Last"
                    }
                }
            });
        },
        error: function () {
            $('#div_feedback').html(GetFeedBacksErrorMarkup());
            iziToast.error({
                title: 'Error',
                message: 'An error occurred in request',
            });
        }
    });
}


function GetSurveyDetail(id_feedback_response_g, client_remotie) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: '../ADMIN/GetSurveyDetail',
        type: 'POST',
        loaderTitle: 'Loading feedback...',
        loaderText: 'We are retrieving the requested surveys..',
        headers: {
            'RequestVerificationToken': token
        },
        data: {
            id_feedback_response_g: id_feedback_response_g
        },
        headers: {
            'RequestVerificationToken': token
        },
        success: function (result) {
            if (client_remotie == 0) {  //REMOTIE
                $('#m_ViewSurveyLabel').text("SURVEY REMOTIE");
            }
            else {
                $('#m_ViewSurveyLabel').text("SURVEY CLIENT");
            }
            $("#div_ViewSurvey").html(result);
            $("#m_ViewSurvey").modal("show");
        },
        error: function () {
            iziToast.error({
                title: 'Error',
                message: 'An error occurred in request',
            });
        }
    });

    
}





