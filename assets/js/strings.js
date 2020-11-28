document.addEventListener("DOMContentLoaded", function(e){
    $.getJSON("/UnofficialCrusaderPatch/assets/doc/strings.json", function(data){
        $(".string").each(function(){
            $(this).html(data[$(this).text()])
        })
    })
    $("html").show()
})