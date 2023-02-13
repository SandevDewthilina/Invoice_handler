//SideMenu related classes
export const SideMenuCategory = (name, menuItems) => {
    return {name: name, menuItems: menuItems}
};
export const MenuItem = (name, icon, active = false) => {
    return {name: name, icon: icon, isActive: active}
};

//Document related classes
export const DocStatus = (completed, total, info) => {
    return {completed: completed, total: total, percentageCompleted: completed / total * 100, info: info}
}
export const LastChange = (date, time) => {
    return {date: date, time: time}
}
export const Document = (subject, to, docStatus, lastChange, checked=false, alert= false) => {
    return {
        checked: checked,
        alert: alert,
        subject: subject,
        to: to,
        status: docStatus,
        lastChange: lastChange
    }
}

export const sideMenuMixin = {
    
    computed:{
        sideMenuCategories() {
            //throw error if _sideMenuCategories is null
            if (this._sideMenuCategories === null) {
                throw new Error("sideMenuCategories is null");
            }
            return this._sideMenuCategories;
        },
        activeMenuItem() {
            //throw error if _activeMenuItem is null
            if (this._activeMenuItem === null) {
                throw new Error("activeMenuItem is null");
            }
            return this._activeMenuItem;
        },
    },
    methods:{
        selectMenuItem(category, menuItem) {
            if (category.name === this._activeMenuItem.categoryName
                && menuItem.name === this._activeMenuItem.itemName) {
                return;
            }
            // deactivate the current active menu item
            this._sideMenuCategories[this._activeMenuItem.categoryName].menuItems
                .find(item => item.name === this._activeMenuItem.itemName).isActive = false;

            // activate the new menu item
            this._activeMenuItem = {categoryName: category.name, itemName: menuItem.name};
            this._sideMenuCategories[category.name].menuItems
                .find(item => item.name === menuItem.name).isActive = true;

        },
    }
    
}