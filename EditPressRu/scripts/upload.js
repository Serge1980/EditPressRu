﻿$(function () {
    // Программное открытие окна выбора файла по щелчку
    $('html').on('click', 'figure', function () {
        //alert('vvv');
        $('#fileUpload').trigger('click');
       // alert('eee');
        //return false;
    })

    // При перетаскивании файлов в форму, подсветить
    $('html').on('dragover', 'section', function (e) {
        $(this).addClass('dd');
        e.preventDefault();
        e.stopPropagation();
    });

    // Предотвратить действие по умолчанию для события dragenter
    $('html').on('dragenter', 'section', function (e) {
        e.preventDefault();
        e.stopPropagation();
    });

    $('html').on('dragleave', 'section', function (e) {
        $(this).removeClass('dd');
    });

    $('html').on('drop', 'section', function (e) {
        if (e.originalEvent.dataTransfer) {
            if (e.originalEvent.dataTransfer.files.length) {
                e.preventDefault();
                e.stopPropagation();

                // Вызвать функцию загрузки. Перетаскиваемые файлы содержатся
                // в свойстве e.originalEvent.dataTransfer.files
                upload(e.originalEvent.dataTransfer.files);
            }
        }
    });

    // Загрузка файлов классическим образом - через модальное окно
    $('html').on('change', '#fileUpload', function () {
        upload($(this).prop('files'));
    });
});

// Функция загрузки файлов
function upload(files) {
    // Создаем объект FormData
    var formData = new FormData();

    // Пройти в цикле по всем файлам
    for (var i = 0; i < files.length; i++) {
        // С помощью метода append() добавляем файлы в объект FormData
        formData.append('file_' + i, files[i]);
    }

    // Ajax-запрос на сервер
    $.ajax({
        type: 'POST',
        url: '/ajax/uploadlogo', // URL на метод действия Upload контроллера HomeController
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function () {
            $('section').removeClass('dd');

            // Перед загрузкой файла удалить старые ошибки и показать индикатор
            $('.error').text('').hide();
            $('.progress').show();

            // Установить прогресс-бар на 0
            $('.progress-bar').css('width', '0');
            $('.progress-value').text('0 %');
        },
        success: function (data, textStatus, XMLHttpRequest) {
            //if (data.Error) {
            //    $('.error').text(data.Error).show();
            //    $('.progress').hide();
            //}
            //else {
            //    $('.progress-bar').css('width', '100%');
            //    $('.progress-value').text('100 %');
            //    $('.progress').delay(5000).hide(0);
            //    $('.section').html(data);
            //    // Отобразить загруженные картинки
            //    //if (data.Files) {
            //    //    // Обертка для картинки со ссылкой
            //    //    var img = '<a href="0" target="_blank"><span style="background-image: url(0)"></span></a>';

            //    //    for (var i = 0; i < data.Files.length; i++) {
            //    //        // Сгенерировать вставляемый элемент с картинкой
            //    //        // (символ 0 заменяем ссылкой с помощью регулярного выражения)
            //    //        var element = $(img.replace(/0/g, data.Files[i]));

            //    //        // Добавить в контейнер
            //    //        $('.images').append(element);
            //    //    }
            //    //}
            //}

                $('.progress-bar').css('width', '100%');
                $('.progress-value').text('100 %');
                $('.progress').delay(5000).hide(0);
                $('.UplRez').html(data);
        },
        xhrFields: { // Отслеживаем процесс загрузки файлов
            onprogress: function (e) {
                if (e.lengthComputable) {
                    // Отображение процентов и длины прогресс бара
                    var perc = e.loaded / 100 * e.total;
                    $('.progress-bar').css('width', perc + '%');
                    $('.progress-value').text(perc + ' %');
                }
            }
        }
    });
}