"use strict";

window.onload = function () {
    $("#day_add_new_participant_form").submit(
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
            var participant_name = document.getElementById('day_add_new_participant_input').value;
            request.open("POST", document.location + "&name=" + participant_name, true);
            request.send(null);

        }
    );

    $(".day_participant_entry_delete").click(
        function () {
            var request = new XMLHttpRequest();
            request.addEventListener("load",
                function (event) {
                    console.log(request.statusText);
                    window.location.reload();
                }
            );
           
            var day_id = $(this).attr("data-day-id");
            var participant_id = $(this).attr("data-participant-id");
            request.open("POST", "/Home/RemoveDay/?dayId=" + day_id + "&participantId=" + participant_id, true);
            request.send(null);
        }
    );

}
