"use strict";


window.onload = function () {

    $("#add_new_participant_form").submit(
        function (e) {
            e.preventDefault();
            var request = new XMLHttpRequest();
            request.addEventListener("load",
                function (event) {
                    if (request.status == 202) {
                        alert(request.statusText);
                    } else {
                        window.location.reload();
                    }
                }
            );
            var participant_name = document.getElementById('add_new_participant_input').value;
            request.open("POST", document.location + "/?name=" + participant_name, true);
            request.send(null);
            
        }
    );   

    


}


