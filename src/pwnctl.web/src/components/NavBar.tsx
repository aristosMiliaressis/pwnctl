import React, { Component } from 'react';

import './NavBar.css';


export class NavBar extends Component<{children: React.ReactNode}>
{
    render() {
        return (
            <div className="sidenav">
                <ul>
                    { this.props.children }
                </ul>
            </div>);
    }
}