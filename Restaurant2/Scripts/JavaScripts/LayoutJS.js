﻿$(document).ready(function () {
	// Requires jQuery

	$(document).on('click', '.js-menu_toggle.closed', function (e) {
		e.preventDefault(); $('.list_load, .list_item').stop();
		$(this).removeClass('closed').addClass('opened');

		$('.side_menu').css({ 'left': '0px' });

		var count = $('.list_item').length;
		$('.list_load').slideDown((count * .6) * 100);
		$('.list_item').each(function (i) {
			var thisLI = $(this);
			timeOut = 100 * i;
			setTimeout(function () {
				thisLI.css({
					'opacity': '1',
					'margin-left': '0'
				});
			}, 100 * i);
		});
	});

	$(document).on('click', '.js-menu_toggle.opened', function (e) {
		e.preventDefault(); $('.list_load, .list_item').stop();
		$(this).removeClass('opened').addClass('closed');

		$('.side_menu').css({ 'left': '-250px' });

		var count = $('.list_item').length;
		$('.list_item').css({
			'opacity': '0',
			'margin-left': '-20px'
		});
		$('.list_load').slideUp(300);
	});
});

