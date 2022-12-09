import React, { Component } from 'react';
import { VersionBanner } from './VersionBanner';
import { StatusBar } from './StatusBar';

export class Footer extends Component
{

    render() {
        return (
            <footer style={{ backgroundColor: '#fff', zIndex: 1 }}>
                <VersionBanner version="0.0.1" />
                <StatusBar />
            </footer>);
    }
}