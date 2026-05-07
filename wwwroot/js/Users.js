function GetRolesSelect() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: '../CAT/GetRolesSelect',
        type: 'POST',
        async: true,
        headers: {
            'RequestVerificationToken': token
        },
        success: function (result) {
            $('#select_rol').html(result);
        },
        error: function () {
            iziToast.error({
                title: 'Error',
                message: 'An error occurred in request',
            });
        }
    });
}


function GetUsersErrorMarkup() {
    return `
            <div class="text-center py-5">
                <i class="fa-solid fa-triangle-exclamation fa-2x text-danger"></i>
                <p class="mt-3 mb-0 fw-semibold">The forms could not be loaded.</p>
                <button type="button" class="btn btn_feedback mt-2" onclick="GetUsers()">Retry</button>
            </div>`;
}


function GetUsers() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: '../USERS/GetUsers',
        type: 'POST',
        loaderTitle: 'Loading users...',
        loaderText: 'We are retrieving the configured users.',
        headers: {
            'RequestVerificationToken': token
        },
        success: function (result) {
            $('#div_users').html(result);
            $('#datatable_users').DataTable({
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
            $('#div_forms').html(GetUsersErrorMarkup());
            iziToast.error({
                title: 'Error',
                message: 'An error occurred in request',
            });
        }
    });
}


function GetDataUser(id_user) {
    AddUpdateForm(1);
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: '../USERS/GetDataUser',
        type: 'POST',
        async: false,
        data: {
            id_user: id_user
        },
        headers: {
            'RequestVerificationToken': token
        },
        success: function (result) {
            if (result == '[]') {
                iziToast.info({
                    title: 'User not found',
                    message: '',
                });
            }
            else {
                var data = JSON.parse(result);
                $("#InputName").val(data[0].Name);
                $("#InputUsername").val(data[0].Username);
                $("#select_rol").val(data[0].IdRol);
                $("#btn_SaveUpdate_User").attr("onclick", "SaveUpdateUser(" + id_user + ");");
                $("#m_AddEditUser").modal("show");
            }
        },
        error: function () {
            iziToast.error({
                title: 'Error',
                message: 'An error occurred in request',
            });
        }
    });
}


function AddUpdateForm(mode) {
    $(".input_clean").val('');
    var btn_SaveUpdate = $("#btn_SaveUpdate_User");
    if (mode == 0) {  //ADD
        $("#m_AddEditUserLabel").text("New user");
        btn_SaveUpdate.text("Save user");
        btn_SaveUpdate.removeClass("btn_feedback");
        btn_SaveUpdate.addClass("btn_feedback_primary");
        btn_SaveUpdate.attr("onclick", "SaveUpdateUser(0);");
    }
    else if (mode == 1) {  //UPDATE
        $("#m_AddEditUserLabel").text("Update user");
        btn_SaveUpdate.text("Update user");
        btn_SaveUpdate.removeClass("btn_feedback_primary");
        btn_SaveUpdate.addClass("btn_feedback");
    }
}


function SaveUpdateUser(id_user) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var name = $("#InputName").val();
    var username = $("#InputUsername").val();
    var id_rol = $("#select_rol").val();

    if (username == '' || name == '') {
        iziToast.warning({
            title: 'Fill all fields',
            message: '',
        });
    }
    else if (id_rol == undefined || id_rol == null) {
        iziToast.warning({
            title: 'Select a rol',
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
            zindex: 9999999,
            title: 'CONFIRMATION',
            message: 'Are you sure about that?',
            position: 'center',
            buttons: [
                ['<button><b>Yes</b></button>', function (instance, toast) {
                    instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
                    $.ajax({
                        url: '../USERS/SaveUpdateUser',
                        type: 'POST',
                        loaderTitle: 'Saving info...',
                        loaderText: '',
                        data: {
                            id_user: id_user,
                            name: name,
                            username: username,
                            id_rol: id_rol
                        },
                        headers: {
                            'RequestVerificationToken': token
                        },
                        success: function (result) {
                            if (result == 0) {
                                GetUsers();
                                if (id_user == 0) {
                                    iziToast.success({
                                        title: "User added",
                                        message: '',
                                    });
                                }
                                else {
                                    iziToast.success({
                                        title: "User updated",
                                        message: '',
                                    });
                                }
                                $("#m_AddEditUser").modal("hide");
                            }
                            else if (result == 2) {
                                iziToast.info({
                                    title: 'Username is already in use',
                                    message: 'Enter another email',
                                });
                            }
                            else {
                                iziToast.error({
                                    title: 'Error',
                                    message: 'An error occurred while saving the information',
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
                ['<button>No. go back</button>', function (instance, toast) {

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


function OnOffUser(id_user, modo) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    iziToast.question({
        timeout: 20000,
        close: false,
        overlay: true,
        displayMode: 'once',
        id: 'question',
        zindex: 999999,
        title: 'CONFIRMATION',
        message: 'Are you sure about that?',
        position: 'center',
        buttons: [
            ['<button><b>Yes</b></button>', function (instance, toast) {
                instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
                $.ajax({
                    url: '../USERS/OnOffUser',
                    type: 'POST',
                    data: {
                        id_user: id_user,
                        modo: modo
                    },
                    headers: {
                        'RequestVerificationToken': token
                    },
                    success: function (result) {
                        if (result == 0) {
                            GetUsers();
                            if (modo == 1) {
                                iziToast.success({
                                    title: "User actived",
                                    message: '',
                                });
                            }
                            else {
                                iziToast.success({
                                    title: "User disabled",
                                    message: '',
                                });
                            }
                        }
                        else if (result == 2) {
                            iziToast.warning({
                                title: "User not found",
                                message: '',
                            });
                        }
                        else {
                            iziToast.error({
                                title: 'Error',
                                message: 'An error occurred while saving changes',
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
            ['<button>No. go back</button>', function (instance, toast) {
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

function ResetUserPassword(id_user) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    iziToast.question({
        timeout: 20000,
        close: false,
        overlay: true,
        displayMode: 'once',
        id: 'question',
        zindex: 999,
        title: 'CONFIRMATION',
        message: 'Are you sure you want to reset the user password?',
        position: 'center',
        buttons: [
            ['<button><b>Yes</b></button>', function (instance, toast) {
                instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
                $.ajax({
                    url: '../USERS/ResetUserPassword',
                    type: 'POST',
                    data: {
                        id_user: id_user
                    },
                    headers: {
                        'RequestVerificationToken': token
                    },
                    success: function (result) {
                        if (result == 0) {
                            iziToast.success({
                                title: "Password reset",
                                message: '',
                            });
                            GetUsers();
                        }
                        else {
                            iziToast.error({
                                title: 'Error',
                                message: 'An error occurred while reset password',
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



