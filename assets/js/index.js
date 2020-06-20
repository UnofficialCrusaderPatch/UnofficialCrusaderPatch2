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