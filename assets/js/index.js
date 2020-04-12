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

$(document).on('click', '.feature-filter', function (e) {
  e.stopPropagation();
  console.log("test");
  if ($("#feature-filter-bugfix")[0].checked != true){
    $("#feature-bugfix-header").addClass("d-none");
    $(".row.feature-bugfix").addClass("d-none");
  } else {
    $("#feature-bugfix-header").removeClass("d-none");
    $(".row.feature-bugfix").removeClass("d-none");
  }
  if ($("#feature-filter-ai")[0].checked != true){
    $("#feature-ai-header").addClass("d-none");
    $(".row.feature-ai").addClass("d-none");
  } else {
    $("#feature-ai-header").removeClass("d-none");
    $(".row.feature-ai").removeClass("d-none");
  }
  if ($("#feature-filter-unit")[0].checked != true){
    $("#feature-unit-header").addClass("d-none");
    $(".row.feature-unit").addClass("d-none");
  } else {
    $("#feature-unit-header").removeClass("d-none");
    $(".row.feature-unit").removeClass("d-none");
  }
  if ($("#feature-filter-misc")[0].checked != true){
    $("#feature-misc-header").addClass("d-none");
    $(".row.feature-misc").addClass("d-none");
  } else {
    $("#feature-misc-header").removeClass("d-none");
    $(".row.feature-misc").removeClass("d-none");
  }
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