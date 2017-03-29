function loadFeed(id)
{
    //alert("Kappa" + id);
    
        
    $('#items').load('ShowFeed/' + id, function () {
        $("#items").slideDown("slow");
        $("#channels").slideUp("slow");
    });
   // $("#items").show(1000);
}

function backToChannels()
{
    $("#channels").slideDown("slow");
    $("#items").slideUp("slow");
  
}