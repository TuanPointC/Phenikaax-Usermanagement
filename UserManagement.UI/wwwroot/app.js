window.ChangeTheme = async (isDark)=>{
    if(isDark){
        document.documentElement.style.setProperty('--background-body', '#212121');
        document.documentElement.style.setProperty('--text-blue', '#3761EE');
        document.documentElement.style.setProperty('--text-gray', '#B0B3B8');
        document.documentElement.style.setProperty('--text-gray-light', '#E4E6EB');
        document.documentElement.style.setProperty('--background-light', '#3A3B3C');
        document.documentElement.style.setProperty('--background-white', '#18191D');
        document.documentElement.style.setProperty('--text-color', 'white');
    }
    else {
        document.documentElement.style.setProperty('--background-body', '#D7D9DD');
        document.documentElement.style.setProperty('--text-blue', '#3761EE');
        document.documentElement.style.setProperty('--text-gray', '#707e89');
        document.documentElement.style.setProperty('--text-gray-light', '#c2c8d5');
        document.documentElement.style.setProperty('--background-light', '#f5f5f6');
        document.documentElement.style.setProperty('--background-white', 'white');
        document.documentElement.style.setProperty('--text-color', 'black');
    }
}
