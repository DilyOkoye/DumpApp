  
    /* Default Notifications */

         function default_noti(errorText){
			Lobibox.notify('default', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
		    position: 'top right',
            msg: errorText
		    });
		  }


function info_noti(errorText){
			Lobibox.notify('info', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
		    position: 'top right',
		    icon: 'fa fa-info-circle',
            msg: errorText
		    });
		  }

function warning_noti(errorText){
			Lobibox.notify('warning', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
		    position: 'top right',
		    icon: 'fa fa-exclamation-circle',
            msg: errorText
		    });
		  }		 

function error_noti(errorText){
			Lobibox.notify('error', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
		    position: 'top right',
		    icon: 'fa fa-times-circle',
		    msg: errorText
		    });
		  }		 

function success_noti(errorText){
			Lobibox.notify('success', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
		    position: 'top right',
		    icon: 'fa fa-check-circle',
		    msg: errorText
		    });
		  }		 




/* Rounded corners Notifications */

function round_default_noti(errorText){
			Lobibox.notify('default', {
		    pauseDelayOnHover: true,
		    size: 'mini',
		    rounded: true,
		    delayIndicator: false,
            continueDelayOnInactiveTab: false,
		    position: 'top right',
            msg: errorText
		    });
		  }


function round_info_noti(errorText){
			Lobibox.notify('info', {
		    pauseDelayOnHover: true,
		    size: 'mini',
		    rounded: true,
		    icon: 'fa fa-info-circle',
		    delayIndicator: false,
            continueDelayOnInactiveTab: false,
		    position: 'top right',
            msg: errorText
		    });
		  }

function round_warning_noti(errorText){
			Lobibox.notify('warning', {
		    pauseDelayOnHover: true,
		    size: 'mini',
		    rounded: true,
		    delayIndicator: false,
		    icon: 'fa fa-exclamation-circle',
            continueDelayOnInactiveTab: false,
		    position: 'top right',
            msg: errorText
		    });
		  }		 

function round_error_noti(errorText){
			Lobibox.notify('error', {
		    pauseDelayOnHover: true,
		    size: 'mini',
		    rounded: true,
		    delayIndicator: false,
		    icon: 'fa fa-times-circle',
            continueDelayOnInactiveTab: false,
		    position: 'top right',
            msg: errorText
		    });
		  }		 

function round_success_noti(errorText){
			Lobibox.notify('success', {
		    pauseDelayOnHover: true,
		    size: 'mini',
		    rounded: true,
		    icon: 'fa fa-check-circle',
		    delayIndicator: false,
            continueDelayOnInactiveTab: false,
		    position: 'top right',
            msg: errorText
		    });
		  }		 




     /* Notifications With Images*/

function img_default_noti(errorText){
			Lobibox.notify('default', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
		    position: 'top right',
		    img: 'assets/plugins/notifications/img/1.jpg', //path to image
            msg: errorText
		    });
		  }


function img_info_noti(errorText){
			Lobibox.notify('info', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
            icon: 'fa fa-info-circle',
		    position: 'top right',
		    img: 'assets/plugins/notifications/img/2.jpg', //path to image
            msg: errorText
            });
		  }

function img_warning_noti(errorText){
			Lobibox.notify('warning', {
		    pauseDelayOnHover: true,
		    icon: 'fa fa-exclamation-circle',
            continueDelayOnInactiveTab: false,
		    position: 'top right',
		    img: 'assets/plugins/notifications/img/3.jpg', //path to image
            msg: errorText
		    });
		  }		 

function img_error_noti(errorText){
			Lobibox.notify('error', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
            icon: 'fa fa-times-circle',
		    position: 'top right',
		    img: 'assets/plugins/notifications/img/4.jpg', //path to image
            msg: errorText
		    });
		  }		 

function img_success_noti(errorText){
			Lobibox.notify('success', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
		    position: 'top right',
		    icon: 'fa fa-check-circle',
		    img: 'assets/plugins/notifications/img/5.jpg', //path to image
            msg: errorText
		    });
		  }		 
		 




     /* Notifications With Images*/


      function pos1_default_noti(errorText){
			Lobibox.notify('default', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
		    position: 'center top',
		    size: 'mini',
            msg: errorText
		    });
		  }

      function pos2_info_noti(errorText){
			Lobibox.notify('info', {
		    pauseDelayOnHover: true,
		    icon: 'fa fa-info-circle',
            continueDelayOnInactiveTab: false,
            position: 'center top',
            size: 'mini',
            hide: true,
            msg: errorText
		    });
		  }

      function pos3_warning_noti(errorText){
			Lobibox.notify('warning', {
		    pauseDelayOnHover: true,
		    icon: 'fa fa-exclamation-circle',
            continueDelayOnInactiveTab: false,
		    position: 'top right',
		    size: 'mini',
            msg: errorText
		    });
		  }		 

      function pos4_error_noti(errorText){
			Lobibox.notify('error', {
		    pauseDelayOnHover: true,
		    icon: 'fa fa-times-circle',
		    size: 'mini',
            continueDelayOnInactiveTab: false,
		    position: 'bottom left',
            msg: errorText
		    });
		  }		 

      function pos5_success_noti(errorText){
			Lobibox.notify('success', {
		    pauseDelayOnHover: true,
		    size: 'mini',
		    icon: 'fa fa-check-circle',
            continueDelayOnInactiveTab: false,
            position: 'center top',
            msg: errorText
		    });
		  }	




     /* Animated Notifications*/


      function anim1_noti(errorText){
			Lobibox.notify('default', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
		    position: 'center top',
		    showClass: 'fadeInDown',
            hideClass: 'fadeOutDown',
            width: 600,
            msg: errorText
		    });
		  }


      function anim2_noti(errorText){
			Lobibox.notify('info', {
		    pauseDelayOnHover: true,
		    icon: 'fa fa-info-circle',
            continueDelayOnInactiveTab: false,
		    position: 'center top',
		    showClass: 'bounceIn',
            hideClass: 'bounceOut',
            width: 600,
            msg: errorText
		    });
		  }

      function anim3_noti(errorText){
			Lobibox.notify('warning', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
            icon: 'fa fa-exclamation-circle',
		    position: 'center top',
		    showClass: 'zoomIn',
            hideClass: 'zoomOut',
            width: 600,
            msg: errorText
		    });
		  }

      function anim4_noti(errorText){
			Lobibox.notify('error', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
            icon: 'fa fa-times-circle',
		    position: 'center top',
		    showClass: 'lightSpeedIn',
            hideClass: 'lightSpeedOut',
            width: 600,
            msg: errorText
		    });
		  }

      function anim5_noti(errorText){
			Lobibox.notify('success', {
		    pauseDelayOnHover: true,
            continueDelayOnInactiveTab: false,
		    position: 'center top',
		    showClass: 'rollIn',
            hideClass: 'rollOut',
            icon: 'fa fa-check-circle',
            width: 600,
            msg: errorText
		    });
		  }