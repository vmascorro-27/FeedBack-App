$(document).ready(function () {
    function updateSlider() {
        const value = $("#scoreRange").val();
        const min = $("#scoreRange").attr("min");
        const max = $("#scoreRange").attr("max");

        const percent = ((value - min) / (max - min)) * 100;

        $("#scoreSelected").text(value);

        $("#scoreRange").css(
            "background",
            `linear-gradient(to right, #4694FF 0%, #4694FF ${percent}%, #e6e6e6 ${percent}%, #e6e6e6 100%)`
        );
    }

    $("#scoreRange").on("input change", function () {
        updateSlider();

        const selectedValue = $(this).val();
        console.log("Valor seleccionado:", selectedValue);
    });

    updateSlider();
});


window.AppLoading = (function () {
    let activeRequests = 0;
    const defaultTitle = 'Loading...';
    const defaultText = 'Please wait a moment.';

    function getElements() {
        const overlay = document.getElementById('global-loading-overlay');
        if (!overlay) {
            return null;
        }

        return {
            overlay,
            title: overlay.querySelector('.global-loading-title'),
            text: overlay.querySelector('.global-loading-text')
        };
    }

    function renderMessage(options) {
        const elements = getElements();
        if (!elements) {
            return;
        }

        elements.title.textContent = options?.title || defaultTitle;
        elements.text.textContent = options?.text || defaultText;
    }

    function syncVisibility() {
        const elements = getElements();
        if (!elements) {
            return;
        }

        const isVisible = activeRequests > 0;
        elements.overlay.classList.toggle('is-visible', isVisible);
        elements.overlay.setAttribute('aria-hidden', isVisible ? 'false' : 'true');
        document.body.classList.toggle('app-loading', isVisible);

        if (!isVisible) {
            renderMessage();
        }
    }

    function show(options) {
        activeRequests++;
        renderMessage(options);
        syncVisibility();
    }

    function hide(force) {
        activeRequests = force ? 0 : Math.max(0, activeRequests - 1);
        syncVisibility();
    }

    function shouldHandleRequest(settings) {
        return settings?.showGlobalLoader !== false;
    }

    $(document).ajaxSend(function (_event, _jqXHR, settings) {
        if (!shouldHandleRequest(settings)) {
            return;
        }

        show({
            title: settings.loaderTitle,
            text: settings.loaderText
        });
    });

    $(document).ajaxComplete(function (_event, _jqXHR, settings) {
        if (!shouldHandleRequest(settings)) {
            return;
        }

        hide();
    });

    return {
        show,
        hide
    };
})();

window.showGlobalLoader = function (options) {
    window.AppLoading.show(options);
};

window.hideGlobalLoader = function (force) {
    window.AppLoading.hide(force);
};


function GetValueResponseQuestion(id_response, value, no_question) {
    $(".stars_" + no_question + "").removeAttr('data-rating');

    var radio = $("#radio_" + id_response + "").prop("checked", true);
    var value_response = $("input[name='rating_" + no_question + "']:checked").val();

    var stars_response = $("#stars_" + id_response + "");
    stars_response.attr("data-rating", value);
}



function AnswerSurvey() {
    const params = new URLSearchParams(window.location.search);
    const client_remotie = params.get("client_remotie");
    const token = params.get("token");
    var id_feedback = $("#id_feedback").val();
    var score_survey = $("#scoreRange").val();

    var valid_responses = false;

    var questions = [];
    var responses = [];
    var count = 0;
    $(".questions_inupts").each(function () {
        var id_question = $(this).attr("id").split('_')[1];
        questions[count] = $(this).val();

        var value_response = null;
        var type_question = $("#typeQuestion_" + id_question + "").val();
        if (type_question == 1) {  //OPTIONABLE
            value_response = $("input[name='rating_" + id_question + "']:checked").val();
        }
        else {
            value_response = $("#openResponse_" + id_question + "").val();
            if (value_response == null) value_response = "";
        }

        if (value_response == null || value_response == undefined) {
            valid_responses = true;
        }
        else {
            responses[count] = value_response;
        }
        count++;
    });



    if (valid_responses == true) {
        iziToast.info({
            title: 'Please answer all questions',
            message: ''
        });
    }
    else if (client_remotie != null) {
        iziToast.warning({
            title: 'Recipient not found',
            message: '',
        });
    }
    else if (id_feedback == undefined || id_feedback == null) {
        iziToast.warning({
            title: 'Feedback pending not found',
            message: '',
        });
    }
    else {
        iziToast.question({
            timeout: 20000,
            close: false,
            overlay: true,
            displayMode: 'once',
            id: 'question',
            zindex: 999,
            title: 'CONFIRMATION',
            message: 'Do you want to send your answers?',
            position: 'center',
            buttons: [
                ['<button><b>Yes</b></button>', function (instance, toast) {
                    instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
                    $.ajax({
                        url: '../FEEDBACK/AnswerSurvey',
                        type: 'POST',
                        loaderTitle: 'Sending responses...',
                        loaderText: '.',
                        data: {
                            token: token,
                            id_feedback: id_feedback,
                            questions: questions,
                            responses: responses,
                            remotie_client: remotie_client,
                            score_survey: score_survey
                        },
                        success: function (result) {
                            if (result == 0) {
                                window.location.reload();
                            }
                            else if (result == 1) {
                                iziToast.error({
                                    title: 'Token invalid',
                                    message: 'Please refresh the page'
                                });
                            }
                            else if (result == 2) {
                                iziToast.error({
                                    title: 'Recipient not found',
                                    message: ''
                                });
                            }
                            else if (result == 3) {
                                iziToast.error({
                                    title: 'Survey dont match with token',
                                    message: 'Please refresh the page'
                                });
                            }
                            else if (result == 4) {
                                iziToast.error({
                                    title: 'Type form not found',
                                    message: ''
                                });
                            }

                            else if (result == -1) {
                                iziToast.error({
                                    title: 'Error',
                                    message: 'An error occurred while saving the survey.'
                                });
                            }
                        },
                        error: function () {
                            console.log('Error en la peticion');
                        }
                    });
                }, true],
                ['<button>No</button>', function (instance, toast) {
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
}