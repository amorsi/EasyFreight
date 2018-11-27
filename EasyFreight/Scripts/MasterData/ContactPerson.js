
$(function () {

    $('body').off('click', '.btnAdd');
    $('body').on('click', '.btnAdd', function () {
        var newItem = $('.form-template').clone(true);
        var lastIndex = parseInt($(this).attr("lastIndex")) + 1;
        $(this).attr("lastIndex", lastIndex);
        newItem.find('input').each(function () {
            this.name = this.name.replace('[-1]', '[' + lastIndex + ']');
            if ($(this).attr("type") != "hidden" && $(this).attr("type") != "checkbox"
                 && !$(this).hasClass("notReq")) {
                $(this).prop('required', true);
            }
        });
        newItem.find('select').each(function () {
            this.name = this.name.replace('[-1]', '[' + lastIndex + ']');
            if (!$(this).hasClass("notReq")) {
                $(this).prop('required', true);
            }
            $(this).select2();
        });
        newItem.insertBefore($(this).parent().parent()).removeClass("form-template").show();

        if (typeof DisableOptions !== 'undefined' && $.isFunction(DisableOptions))
            DisableOptions();

    });

    $(document).off('click', '.remove-contact');
    $(document).on('click', '.remove-contact', function () {
        $(this).parents('.contactform').slideUp("normal", function () {
            $(this).remove();

            

            var lastIndex = parseInt($('.btnAdd').attr("lastIndex"));
            //reset all divs index
            $($('.contactform').get().reverse()).each(function () {
                var divContent = $(this);
                var firstInput = $(this).find('input.form-control').first().attr('name');
                if (firstInput.indexOf("[-1]") == -1 && firstInput.indexOf("[0]") == -1
                    && firstInput.indexOf("[") != -1 && lastIndex > 0) {
                    var xx = lastIndex - 1;
                    divContent.find('input').each(function () {
                        if (this.name.indexOf("[-1]") == -1 && this.name.indexOf("[0]") == -1
                            && this.name.indexOf("[") != -1 && lastIndex > 0) {

                            this.name = this.name.replace('[' + lastIndex + ']', '[' + xx + ']');
                        }
                    });

                    divContent.find('select').each(function () {
                        if (this.name.indexOf("[-1]") == -1 && this.name.indexOf("[0]") == -1
                            && this.name.indexOf("[") != -1 && lastIndex > 0) {

                            this.name = this.name.replace('[' + lastIndex + ']', '[' + xx + ']');
                        }
                    });

                    lastIndex--;

                }

            });

            var newIndex = parseInt($('.btnAdd').attr("lastIndex")) - 1;
            $('.btnAdd').attr("lastIndex", newIndex);

            if (typeof DisableOptions !== 'undefined' && $.isFunction(DisableOptions))
                DisableOptions();

        });

        

    });

});

