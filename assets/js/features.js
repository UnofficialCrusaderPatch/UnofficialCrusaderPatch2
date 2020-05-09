$(document).on('click', '.feature-filter', function (e) {
    e.stopPropagation();

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