$(".list-group-item").on("click", function() {
    $(".list-group-item").removeClass("active");
    $(this).addClass("active");
  });

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