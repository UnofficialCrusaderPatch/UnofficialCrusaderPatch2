window.onload = function(e){
    $.getJSON("assets/doc/faq.json", function(data){
        var faq = document.getElementById('faq-alt');
        faq.classList.add('d-none');

        var faq = document.getElementById('faq-content');
        var items = []
        $.each( data, function( index, item ) {
            let question = document.createElement('div');
            question.classList.add('card');
            question.classList.add('faq-question');

            let header = document.createElement('h5');
            header.innerHTML = item['question'];

            let divider = document.createElement('hr');
            divider.classList.add('faq-question-divider');

            let answer = document.createElement('div');
            answer.classList.add('card-body');
            answer.classList.add('faq-question-body');
            answer.innerHTML = item['answer'];

            question.appendChild(header);
            question.appendChild(divider);
            question.appendChild(answer);
            
            faq.appendChild(question);
        })
    });
}