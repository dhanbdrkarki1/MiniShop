// product increment and decrement
$(document).ready(function () {

	var quantitiy = 0;
	$('.quantity-right-plus').click(function (e) {

		// Stop acting like a button
		e.preventDefault();
		// Get the field name
		var quantity = parseInt($('#quantity').val());

		// If is not undefined

		$('#quantity').val(quantity + 1);


		// Increment

	});

	$('.quantity-left-minus').click(function (e) {
		// Stop acting like a button
		e.preventDefault();
		// Get the field name
		var quantity = parseInt($('#quantity').val());

		// If is not undefined

		// Increment
		if (quantity > 0) {
			$('#quantity').val(quantity - 1);
		}
	});

});



// text ellipsis
document.addEventListener("DOMContentLoaded", function () {
	var elements = document.querySelectorAll(".truncated-text");
	var maxLength = 55;

	elements.forEach(function (element) {
		if (element.textContent.length > maxLength) {
			var truncatedText = element.textContent.substring(0, maxLength) + " ...";
			element.textContent = truncatedText;
			element.title = element.dataset.originalText;
		}
	});
});