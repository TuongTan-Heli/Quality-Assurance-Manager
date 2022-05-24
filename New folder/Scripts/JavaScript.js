function ChangeToReply(Id) {
    var c = $("[name=Comment]");
    var a = $("[name=IdeaId]");
    var b = $("[name=CommentId]");
    $(a).attr("value", "0");
    $(b).attr("value", Id);

    $(b).attr("placeholder","Replying to");
   
    
}
function ChangeToComment(Id) {

    var a = $("[name=IdeaId]");
    var b = $("[name=CommentId]");
    $(a).attr("value", Id);
    $(b).attr("value", "0");




}


function Show(x,IdeaId) {
   
    var a = $(x).next("tr");
    if ($(a).css("display") == "none") {
        $(a).show();
        //Add View
        var data = {};
        data.IdeaId = IdeaId;
        $.ajax({
            type: "POST",
            url: "/Ideas/AddView",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (r) {
                

            },
            error: function () {
                alert("Failed, check information again");
            }
        });
    }
    else {
        $(a).hide();
    }
    
}
$("body").on("submit", "#CreateForm", function () {
    return confirm("Do you Agree with out Terms & Conditions before submit?");
});
