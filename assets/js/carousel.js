window.onload = function() { 
    carouselNormalization(); 
}

function carouselNormalization() {
    var items = $('#carouselInstallation .carousel-item'), //grab all slides
      heights = [], //create empty array to store height values
      tallest; //create variable to make note of the tallest slide
  
    if (items.length) {
      function normalizeHeights() {
        items.each(function() { //add heights to array
          heights.push($(this).height());
        });
        tallest = Math.max.apply(null, heights); //cache largest value
        items.each(function() {
          $(this).css('min-height', tallest + 'px');
        });
        $(".carousel-control-prev-icon").each(function(){
            $(this).css('position', 'absolute');
            $(this).css('top', tallest/2 + $(this).height());
        })
        $(".carousel-control-next-icon").each(function(){
            $(this).css('position', 'absolute');
            $(this).css('top', tallest/2 + $(this).height());
        })
      };
      normalizeHeights();
  
      $(window).on('resize orientationchange', function() {
        tallest = 0, heights.length = 0; //reset vars
        items.each(function() {
          $(this).css('min-height', '0'); //reset min-height
        });
        normalizeHeights(); //run it again 
      });
    }
  }