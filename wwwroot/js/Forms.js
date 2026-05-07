function GetFormsErrorMarkup() {
    return `
            <div class="text-center py-5">
                <i class="fa-solid fa-triangle-exclamation fa-2x text-danger"></i>
                <p class="mt-3 mb-0 fw-semibold">The forms could not be loaded.</p>
                <button type="button" class="btn btn_feedback mt-2" onclick="GetForms()">Retry</button>
            </div>`;
}

function GetForms() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: '../SURVEYS/GetForms',
        type: 'POST',
        loaderTitle: 'Loading forms...',
        loaderText: 'We are retrieving the configured forms.',
        headers: {
            'RequestVerificationToken': token
        },
        success: function (result) {
            $('#div_forms').html(result);
            $('#datatable_surveys').DataTable({
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
            $('#div_forms').html(GetFormsErrorMarkup());
            iziToast.error({
                title: 'Error',
                message: 'An error occurred in request',
            });
        }
    });
}

function ShowSurvey(id_form_g) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: '../SURVEYS/ShowSurvey',
        type: 'POST',
        loaderTitle: 'Loading survey...',
        loaderText: 'We are preparing the survey preview.',
        data: {
            id_form_g: id_form_g
        },
        headers: {
            'RequestVerificationToken': token
        },
        success: function (result) {
            $('#div_ViewForm').html(result);
            $("#m_ViewForm").modal("show");
        },
        error: function () {
            iziToast.error({
                title: 'Error',
                message: 'An error occurred in request',
            });
        }
    });
}

function GetFormTypesSelect() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: '../CAT/GetFormTypesSelect',
        type: 'POST',
        async: false,
        headers: {
            'RequestVerificationToken': token
        },
        success: function (result) {
            $('#id_type_form').html(result);
        },
        error: function() {
            iziToast.error({
                title: 'Error',
                message: 'An error occurred in request',
            });
        }
    });
}

function ShowResponsesForm(index) {  
        //MODE 1: OPTIONALBLE     MODE 0: WRITTEN
        var mode = document.getElementById("SwitchQuestion_"+ index +"").checked ? 1 : 0;

        if(mode == 1){
            $("#divFormResponses_"+ index +"").css("display", "block");
        }
        else{
            $("#divFormResponses_"+ index +"").css("display", "none");
        }
    }

function AddQuestion(){
    count_question++;
    var newQuestion = "<div id='divQuestion_"+ count_question +"' class='row card-question mt-3' draggable='true' ondragend='dragEnd()' ondragover='dragOver(event)' ondragstart='dragStart(event)'>" +
        "<div class='col-md-5'><h5 class='text-center'>Question</h5><textarea placeholder='New Question' class='form-control' rows='3' id='txt_question_"+ count_question +"'></textarea></div>" +
            
        "<div class='col-md-4' id='divFormResponses_"+ count_question +"' style='display:none;'>" +
            "<h5 class='text-center'>Responses</h5>" +
            "<div class='row'>" +
                "<div class='col-md-11' id='divResponses_"+ count_question +"'+  +''>" +
                    "<input class='form-control reponse_"+ count_question +" responses_question_"+ count_question +"' placeholder='New response' />" +
                "</div>" +
                "<div class='col-md-1 text-start'>" +
                    "<button class='btn btn-sm btn-success' onclick='AddResponse("+ count_question +");'><i class='fa fa-plus'></i></button>" +
                "</div>" +
            "</div>" +
        "</div>" +

        "<div class='col-md-3 text-center'>" +
            "<label class='switch'>" +
                "<input type='checkbox' id='SwitchQuestion_"+ count_question +"' onchange='ShowResponsesForm("+ count_question +")' />" +
                "<span class='slider'></span>" +
            "</label>" +
            "<span> Written / Optionable </span>" +
            "<button class='btn btn-sm btn-danger btn-remove-question' onclick='RemoveQuestion("+ count_question +");'><i class='fa fa-remove'></i></button>" +
        "</div>" +
            
    "</div>";

    $("#div_form").append(newQuestion);
}

function RemoveQuestion(index_question){
    $("#divQuestion_"+ index_question +"").remove();
}


function AddResponse(index_question){
    var index_response = Math.floor(Math.random() * 1000) + 1;
    var new_response = "<div class='row' id='response_"+ index_question +"_"+ index_response +"'>" +
                "<div class='col-md-11' id='divResponses_"+ index_question +"_"+ index_response +"'>" +
                    "<input class='form-control reponse_"+ index_question +"_"+ index_response+" responses_question_"+ index_question +"' placeholder='New response' />" +
                "</div>" +
                "<div class='col-md-1 text-start'>" +
                    "<button class='btn btn-sm btn-danger' onclick='RemoveResponse("+ index_question +", "+ index_response +");'><i class='fa fa-remove'></i></button>" +
                "</div>" +
            "</div>";

    $("#divFormResponses_"+ index_question +"").append(new_response);
    $(".reponse_"+ index_question +"").focus();
}

function RemoveResponse(index_question, index_response){
    $("#response_"+ index_question +"_"+ index_response +"").remove();
}


function SaveForm() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var id_form_g = $("#id_form_g").val();
    if(id_form_g == '' || id_form_g == undefined){ id_form_g = null; }

    var name_form = $("#txt_name").val();
    var id_type_form = $("#id_type_form").val();

    var valid_question = false;
    var valid_responses = false;
    var valid_count_responses = false;

    var questions = [];
    var types_question = [];
    var responses = [];
    var count = 0;

    var question_box = $(".card-question");
    question_box.each(function(){
        var id_question = $(this).attr("id").split('_')[1];
        var question = $("#txt_question_"+ id_question +"").val();
        if(question == '' || question.trim().length == 0){
            valid_question = true;
        }
        questions[count] = question;

        var type = document.getElementById("SwitchQuestion_"+ id_question +"").checked ? 1 : 0;
        types_question[count] = type;
        responses[count] = [];

        if(type == 1){  //OPTIONABLE
            var count_response = 0;
            var responses_question = $(".responses_question_"+ id_question +"");
            if(responses_question.length <= 1){
                valid_count_responses = true;
            }
            responses_question.each(function(){
                if($(this).val() == '' || $(this).val().trim().length == 0){ valid_responses = true; }
                responses[count][count_response] = $(this).val();
                count_response++;
            });
        }
        else{
            responses[count][0] = "";
        }
        count++;
    });

    if(name_form == '' || name_form.trim().length == 0){
        iziToast.info({
            title: 'Enter a valid name',
            message: '',
        });
        return;
    }
    else if(id_type_form == undefined || id_type_form == ''){
        iziToast.info({
            title: 'Type form dont detected',
            message: '',
        });
        return;
    }
    else if(questions.length == 0){
        iziToast.info({
            title: 'Enter at least 1 question',
            message: '',
        });
    }
    else if(valid_question == true){
        iziToast.info({
            title: 'Fill all questions',
            message: '',
        });
        return;
    }
    else if(valid_responses == true){
        iziToast.info({
            title: 'Fill all responses',
            message: '',
        });
        return;
    }
    else if(valid_count_responses == true){
        iziToast.info({
            title: 'Enter more than 1 answer',
            message: '',
        });
        return;
    }
    else{
        iziToast.question({
            timeout: 20000,
            close: false,
            overlay: true,
            displayMode: 'once',
            id: 'question',
            zindex: 999,
            title: 'CONFIRMATION',
            message: '¿Are you sure you want to save the survey??',
            position: 'center',
            buttons: [
                ['<button><b>YES</b></button>', function (instance, toast) {
                    $.ajax({
                        url: '../SURVEYS/SaveForm',
                        type: 'POST',
                        asyn: false,
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({
                            id_form_g:id_form_g,
                            nameForm: name_form,
                            idTypeForm: id_type_form,
                            questions: questions,
                            typeQuestion: types_question,
                            responses: responses
                        }),
                        headers: {
                            'RequestVerificationToken': token
                        },
                        success: function(result) {
                            if(result == 0){
                                if (id_form_g == null) {
                                    window.location.href = '/SURVEYS/Forms?msj=Survery saved correctly';
                                }
                                else { window.location.href = '/SURVEYS/Forms?msj=Survery update correctly'; }
                            }
                            else if (result == 1) {
                                iziToast.info({
                                    title: 'A form of this type already exists. ',
                                    message: 'Please select a different form type.',
                                });
                            }
                            else{
                                iziToast.error({
                                    title: 'Error',
                                    message: 'An error occurred while saving the survey.',
                                });
                            }

                        },
                        error: function(xhr) {
                            iziToast.error({
                                title: 'Error',
                                message: 'An error occurred in request',
                            });
                        }
                    });
                    instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');

                }, true],
                ['<button>No, go ack</button>', function (instance, toast) {

                    instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');

                }],
            ],
            onClosing: function(instance, toast, closedBy){
                console.info('Closing | closedBy: ' + closedBy);
            },
            onClosed: function(instance, toast, closedBy){
                console.info('Closed | closedBy: ' + closedBy);
            }
        });
    }
}


function OnOffForm(id_form_g) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    iziToast.question({
        timeout: 20000,
        close: false,
        overlay: true,
        displayMode: 'once',
        id: 'question',
        zindex: 999,
        title: 'CONFIRMATION',
        message: 'Are you sure about that?',
        position: 'center',
        buttons: [
            ['<button><b>Yes, remove survey</b></button>', function (instance, toast) {
                instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
                $.ajax({
                    url: '../SURVEYS/OnOffForm',
                    type: 'POST',
                    async: true,
                    data: {
                        id_form_g: id_form_g
                    },
                    headers: {
                        'RequestVerificationToken': token
                    },
                    success: function (result) {
                        if (result == 0) {
                            iziToast.success({
                                title: 'Survey successfully deactivated',
                                message: '',
                            });
                            GetForms();
                        }
                        else if (result == 1) {
                            iziToast.warning({
                                title: 'Survey dont found',
                                message: '',
                            });
                        }
                        else {
                            iziToast.error({
                                title: 'There was an error in the request',
                                message: '',
                            });
                        }
                    },
                    error: function () {
                        iziToast.error({
                            title: 'Error',
                            message: 'An error occurred in request',
                        });
                    }
                });
            }, true],
            ['<button>No, go back</button>', function (instance, toast) {
                instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
            }],
        ],
        onClosing: function (instance, toast, closedBy) {
            console.info('Closing | closedBy: ' + closedBy);
        },
        onClosed: function (instance, toast, closedBy) {
            console.info('Closed | closedBy: ' + closedBy);
        }
    });
}



    // #region DRAG & DROP QUESTIONS
    let selected = null
    function dragOver(e) {
      if (isBefore(selected, e.target)) {
        e.target.parentNode.insertBefore(selected, e.target);
      } else {
          try{
              e.target.parentNode.insertBefore(selected, e.target.nextSibling);
          }
          catch{}
      }
    }

    function dragEnd() {
      selected = null
    }

    function dragStart(e) {
      e.dataTransfer.effectAllowed = 'move'
      e.dataTransfer.setData('text/plain', null)
      selected = e.target
    }

    function isBefore(el1, el2) {
      let cur
      if (el2.parentNode === el1.parentNode) {
        for (cur = el1.previousSibling; cur; cur = cur.previousSibling) {
          if (cur === el2) return true
        }
      }
      return false;
    }

    // #endregion