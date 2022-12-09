import React, { Component } from 'react';
import { Logo } from './Logo';

export class Header extends Component<{logo: string}>
{
    render() {
        return (
            <div className="header">
                <Logo logo={this.props.logo}/>
            </div>);
    }
}