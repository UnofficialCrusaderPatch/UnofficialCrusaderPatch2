// $('.collapse').on('show.bs.collapse', function(e) {
//   var $card = $(this).closest('.card');
//   var $open = $($(this).data('parent')).find('.collapse.show');

//   if (e.target.id == "navbarSupportedContent") return;

//   var additionalOffset = 0;
//   if($card.prevAll().filter($open.closest('.card')).length !== 0)
//   {
//         additionalOffset =  $open.height();
//   }
//   $('html,body').animate({
//     scrollTop: $card.offset().top - additionalOffset
//   }, 500);
// });

$(".list-group-item").on("click", function() {
  $(".list-group-item").removeClass("active");
  $(this).addClass("active");
});

// var aicFieldList = $.getJSON("res/fields.json");
// aicFields = aicFieldList.responseJSON;
// data = data.responseJSON
// aic = {}
// data.forEach(function(item, index){
//   console.log(item);
//   aic[item] = $("#" + item)[0].value;
// });

//download(JSON.stringify(aic, undefined, 4), "text.txt", "text\plain")

function download(data, filename, type) {
  var file = new Blob([data], {type: type});
  if (window.navigator.msSaveOrOpenBlob) // IE10+
      window.navigator.msSaveOrOpenBlob(file, filename);
  else { // Others
      var a = document.createElement("a"),
              url = URL.createObjectURL(file);
      a.href = url;
      a.download = filename;
      document.body.appendChild(a);
      a.click();
      setTimeout(function() {
          document.body.removeChild(a);
          window.URL.revokeObjectURL(url);  
      }, 0); 
  }
}


$(function(){
    $("#aic-table").load("aic-description.html"); 
});

$(function(){
  $("#aic-popularity").load("res/popularity.html"); 
});

$(function(){
  $("#aic-taxes").load("res/taxes.html"); 
});

$(function(){
  $("#aic-food").load("res/food.html"); 
});

$(function(){
  $("#aic-farms").load("res/farms.html"); 
});

$(function(){
  $("#aic-buildings").load("res/buildings.html"); 
});

$(function(){
  $("#aic-build-efficiency").load("res/build-efficiency.html"); 
});

$(function(){
  $("#aic-resource-management").load("res/resource-management.html"); 
});

$(function(){
  $("#aic-weapons").load("res/weapons.html"); 
});

$(function(){
  $("#aic-recruiting").load("res/recruiting.html"); 
});

$(function(){
  $("#aic-attacking").load("res/attacking.html"); 
});

$(function(){
  $("#aic-attack-siege").load("res/attack-siege.html"); 
});

$(function(){
  $("#aic-defense").load("res/defense.html"); 
});

$(function(){
  $("#aic-uncategorized").load("res/uncategorized.html"); 
});